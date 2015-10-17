using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Services
{
    public interface IDataAccessService
    {
        void SaveProject(ProjectDocument project);
        void SaveStage(StageDocument stage);
        void SaveTileset(TilesetDocument tileset);
    }
}
