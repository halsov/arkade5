using System.IO;
using Arkivverket.Arkade.Util;

namespace Arkivverket.Arkade.Core
{
    public class Archive
    {
        public Uuid Uuid { get; private set; }
        public WorkingDirectory WorkingDirectory { get; private set; }
        public ArchiveType ArchiveType { get; private set; }

        public Archive(ArchiveType archiveType, Uuid uuid, WorkingDirectory workingDirectory)
        {
            ArchiveType = archiveType;
            Uuid = uuid;
            WorkingDirectory = workingDirectory;
        }

        public string GetContentDescriptionFileName()
        {
            return WorkingDirectory.Content().WithFile(ArkadeConstants.ArkivstrukturXmlFileName).FullName;
        }

        public string GetStructureDescriptionFileName()
        {
            return WorkingDirectory.AdministrativeMetadata().WithFile(ArkadeConstants.AddmlXmlFileName).FullName;
        }

        public string GetContentDescriptionXmlSchemaFileName()
        {
            return WorkingDirectory.Content().WithFile(ArkadeConstants.ArkivstrukturXsdFileName).FullName;
        }

        public string GetStructureDescriptionXmlSchemaFileName()
        {
            return WorkingDirectory.Content().WithFile(ArkadeConstants.AddmlXsdFileName).FullName;
        }

        public string GetMetadataCatalogXmlSchemaFileName()
        {
            return WorkingDirectory.Content().WithFile(ArkadeConstants.MetadatakatalogXsdFileName).FullName;
        }

        public bool HasStructureDescriptionXmlSchema()
        {
            return File.Exists(GetStructureDescriptionXmlSchemaFileName());
        }

        public bool HasContentDescriptionXmlSchema()
        {
            return File.Exists(GetContentDescriptionXmlSchemaFileName());
        }

        public bool HasMetadataCatalogXmlSchema()
        {
            return File.Exists(GetMetadataCatalogXmlSchemaFileName());
        }

        public FileInfo GetInformationPackageFileName()
        {
            return WorkingDirectory.Root().WithFile(Uuid.ToString() + ".tar");
        }
    }

    public enum ArchiveType
    {
        Noark3,
        Noark4,
        Noark5,
        Fagsystem,
    }
}