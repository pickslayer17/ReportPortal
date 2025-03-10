﻿using AutoMapper;
using ReportPortal.BL.Constatnts;
using ReportPortal.BL.Models;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Exceptions;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories.Interfaces;
using System.Linq.Expressions;

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

        public async Task<IEnumerable<FolderDto>> GetAllFolders(int runId, CancellationToken cancellationToken = default)
        {
            var folders = await _folderRepository.GetAllByAsync(f => f.RunId == runId, cancellationToken);
            var folderDto = folders.Select(f => _mapper.Map<FolderDto>(f));

            return folderDto;
        }

        public async Task<int> DoesFolderExists(int runId, string path)
        {
            var run = await _runRepository.GetByAsync(r => r.Id == runId);
            if (run == null) throw new DirectoryNotFoundException($"There is no run with such id {runId}!");

            var folderNames = path.ToLower().Split('.');
            if (folderNames.Length == 0) throw new DirectoryNotFoundException($"Test cannot be added without directory.");

            Folder rootFolder;
            rootFolder = run.Folders.FirstOrDefault(f => f.FolderLevel == 0);
            if (rootFolder == null)
            {
                throw new Exception($"Critical app error - no root folder for the run with id {runId}");
            }

            var folder = await GetFolder(rootFolder, folderNames);

            return folder.Id;
        }

        private async Task<Folder> GetFolder(Folder parentFolder, string[] folderNames)
        {
            var currentFolderName = folderNames[0];
            var currentFolder = parentFolder.Children.FirstOrDefault(f => f.Name.ToLower() == currentFolderName);
            if (currentFolder == null)
                throw new FolderNotFoundException(
                    $"Folder with name {currentFolderName} was not found in parent folder {parentFolder.Name} id: {parentFolder.Id}");

            if (folderNames.Length == 1)
            {
                return currentFolder;
            }
            else
            {
                return await GetFolder(currentFolder, folderNames.Skip(1).ToArray());
            }
        }

        public async Task<int> GetIdOrAddFolderInRun(int runId, string path)
        {
            var run = await _runRepository.GetByAsync(r => r.Id == runId);
            if (run == null) throw new DirectoryNotFoundException($"There is no run with such id {runId}!");

            var folderNames = path.ToLower().Split('.');
            if (folderNames.Length == 0) throw new DirectoryNotFoundException($"Test cannot be added without directory.");

            Folder rootFolder;
            rootFolder = run.Folders.FirstOrDefault(f => f.Name == FolderNames.RootFolderName);
            if (rootFolder == null)
            {
                throw new Exception($"No root folder for run id {run.Id}");
            }

            return await GetIdOrAddFolder(rootFolder, run.Id, 1, folderNames);
        }

        private async Task<int> GetIdOrAddFolder(Folder parentFolder, int runId, int folderLevel, string[] folderNames)
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
                    var newFolder = await CreateFolder(parentFolder, runId, currentFolderName, folderLevel);

                    return newFolder.Id;
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
                    childFolder = await CreateFolder(parentFolder, runId, currentFolderName, folderLevel);
                }

                folderNames = folderNames.Skip(1).ToArray();

                return await GetIdOrAddFolder(childFolder, runId, ++folderLevel, folderNames);
            }
        }

        public async Task CreateRootFolder(int runId)
        {
            bool exists = await _folderRepository.ExistsAsync(f => f.Name == FolderNames.RootFolderName && f.RunId == runId);

            if (!exists)
            {
                var rootFolder = await CreateFolder(null, runId, FolderNames.RootFolderName, 0);
            }
            else
            {
                throw new Exception($"Run id {runId} already has a root folder");
            }
        }

        private async Task<Folder> CreateFolder(Folder parentFolder, int runId, string folderName, int folderLevel)
        {
            var folder = new Folder
            {
                Name = folderName,
                RunId = runId,
                FolderLevel = folderLevel,
                Parent = parentFolder
            };

            await _folderRepository.InsertAsync(folder);

            return folder;
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

        public async Task DeleteFolder(int folderId)
        {
            var folder = await GetByIdAsync(folderId);

            foreach (var child in folder.Children)
            {
                await DeleteFolder(child.Id);
            }

            await _folderRepository.RemoveByIdAsync(folderId);
        }
    }
}
