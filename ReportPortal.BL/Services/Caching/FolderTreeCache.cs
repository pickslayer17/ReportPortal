using System.Collections.Concurrent;

namespace ReportPortal.BL.Services.Caching
{
    /// <summary>
    /// Process-wide cache of each run's folder tree (single backend, single DB).
    /// Folder mutations go through FolderService, which keeps this cache in sync
    /// (write-through on add, eviction on delete). Navigation/display still reads
    /// the DB directly, so it always reflects the current state.
    /// </summary>
    public interface IFolderTreeCache
    {
        bool TryGet(int runId, out RunFolderTree tree);
        RunFolderTree GetOrAdd(int runId, RunFolderTree tree);
        void Invalidate(int runId);
    }

    public class FolderTreeCache : IFolderTreeCache
    {
        private readonly ConcurrentDictionary<int, RunFolderTree> _trees = new();

        public bool TryGet(int runId, out RunFolderTree tree) => _trees.TryGetValue(runId, out tree);

        public RunFolderTree GetOrAdd(int runId, RunFolderTree tree) => _trees.GetOrAdd(runId, tree);

        public void Invalidate(int runId) => _trees.TryRemove(runId, out _);
    }

    /// <summary>
    /// One run's folder tree: maps a lowercased dotted path (relative to the root,
    /// "" = root) to a folder id. The gate serialises create operations so two
    /// concurrent uploads cannot insert the same folder twice.
    /// </summary>
    public class RunFolderTree
    {
        private readonly ConcurrentDictionary<string, int> _idByPath;

        public int RootId { get; }
        public SemaphoreSlim Gate { get; } = new(1, 1);

        public RunFolderTree(int rootId, IDictionary<string, int> idByPath)
        {
            RootId = rootId;
            _idByPath = new ConcurrentDictionary<string, int>(idByPath, StringComparer.Ordinal);
        }

        public bool TryResolve(string lowerPath, out int id) => _idByPath.TryGetValue(lowerPath, out id);

        public void Set(string lowerPath, int id) => _idByPath[lowerPath] = id;

        /// <summary>Removes a folder path and every descendant path.</summary>
        public void Remove(string lowerPath)
        {
            _idByPath.TryRemove(lowerPath, out _);
            var prefix = lowerPath + ".";
            foreach (var key in _idByPath.Keys)
            {
                if (key.StartsWith(prefix, StringComparison.Ordinal))
                    _idByPath.TryRemove(key, out _);
            }
        }
    }
}
