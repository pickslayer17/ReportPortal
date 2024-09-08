using AutoMapper;
using ReportPortal.BL.Constatnts;
using ReportPortal.BL.Models;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Exceptions;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories.Interfaces;
using System.Threading;

namespace ReportPortal.BL.Services
{
    public class FolderService : IFolderService
    {
        private readonly IFolderRepository _folderRepository;
        private readonly IRunRepository _runRepository;
        private readonly IMapper _mapper;

        public FolderService(IRunRepository runRepository, IFolderRepository folderRepository, IMapper mapper)
        {
            _runRepository = runRepository;
            _folderRepository = folderRepository;
            _mapper = mapper;
        }

        public async Task<int> GetIdOrAddFolderInRun(int runId, string path)
        {
            var run = await _runRepository.GetByAsync(r => r.Id == runId);
            if (run == null) throw new DirectoryNotFoundException($"There is no run with such id {runId}!");

            var folderNames = path.Split('.');
            if (folderNames.Length == 0) throw new DirectoryNotFoundException($"Test cannot be added without directory.");

            var rootFolder = run.RootFolder;

            return await GetIdOrAddFolder(rootFolder.ToFolder(), folderNames);
        }

        private async Task<int> GetIdOrAddFolder(Folder parentFolder, string[] folderNames)
        {
            var currentFolderName = folderNames[0];
            if (folderNames.Length == 1)
            {
                if (parentFolder.Children != null && parentFolder.Children.Any(c => c.Name == currentFolderName))
                {
                    return parentFolder.Children.First(c => c.Name == currentFolderName).Id;
                }
                else
                {
                    return await CreateFolder(parentFolder, currentFolderName);
                }
            }
            else
            {
                Folder childFolder = null;
                if (parentFolder.Children != null && parentFolder.Children.Any(c => c.Name == currentFolderName))
                {
                    childFolder = parentFolder.Children.First(c => c.Name == currentFolderName);
                }
                else
                {
                    var newFolderId = await CreateFolder(parentFolder, currentFolderName);
                    childFolder = await _folderRepository.GetByAsync(f => f.Id == newFolderId);
                }

                folderNames = folderNames.Skip(1).ToArray();

                return await GetIdOrAddFolder(childFolder, folderNames);
            }
        }

        private async Task<int> CreateFolder(Folder parentFolder, string folderName)
        {
            var folder = new Folder
            {
                Name = folderName,
                RunId = parentFolder.RunId,
                Parent = parentFolder
            };

            var folderId = await _folderRepository.InsertAsync(folder);

            return folderId;
        }

        public async Task AttachTestToFolder(int folderId, int testId)
        {
            throw new NotImplementedException();
        }

        public async Task<FolderDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var folderRunItem = await _folderRepository.GetByAsync(f => f.Id == id, cancellationToken);
            if (folderRunItem == null) throw new FolderNotFoundException($"folder with id {id} was not found");

            return _mapper.Map<FolderDto>(folderRunItem);
        }
    }
}
