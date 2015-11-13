using System;
using MegaMan.Common;

namespace MegaMan.Editor.Bll.Factories
{
    public interface IStageDocumentFactory
    {
        StageDocument CreateNew(FilePath filePath);
        StageDocument Load(ProjectDocument project, StageLinkInfo linkInfo);
    }
}
