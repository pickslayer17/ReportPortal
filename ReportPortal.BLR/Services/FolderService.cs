﻿using AutoMapper;
using ReportPortal.BL.Models;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Exceptions;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories.Interfaces;

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

            return await GetIdOrAddFolder(null, run, 0, folderNames);
        }

        private async Task<int> GetIdOrAddFolder(Folder parentFolder, Run run, int folderLevel, string[] folderNames)
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
                    return await CreateFolder(parentFolder, run, currentFolderName, ++folderLevel);
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
                    var newFolderId = await CreateFolder(parentFolder, run, currentFolderName, ++folderLevel);
                    childFolder = await _folderRepository.GetByAsync(f => f.Id == newFolderId);
                }

                folderNames = folderNames.Skip(1).ToArray();

                return await GetIdOrAddFolder(childFolder, run, ++folderLevel, folderNames);
            }
        }

        private async Task<int> CreateFolder(Folder parentFolder, Run run, string folderName, int folderLevel)
        {
            var folder = new Folder
            {
                Name = folderName,
                RunId = run.Id,
                FolderLevel = folderLevel,
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
