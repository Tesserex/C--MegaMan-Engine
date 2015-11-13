using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaMan.Common;

namespace MegaMan.Editor.Bll.Factories
{
    public class StageDocumentFactory : IStageDocumentFactory
    {
        private readonly FactoryCore _core;

        public StageDocumentFactory(FactoryCore core)
        {
            _core = core;
        }

        public StageDocument CreateNew(FilePath filePath)
        {
            throw new NotImplementedException();
        }

        public StageDocument Load(ProjectDocument project, StageLinkInfo linkInfo)
        {
            var reader = _core.Reader.GetStageReader(linkInfo.StagePath);
            var stage = reader.Load(linkInfo.StagePath);
            var document = new StageDocument(project, stage, linkInfo);
            return document;
        }
    }
}
