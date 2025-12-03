
using Newtonsoft.Json.Linq;

namespace Code.Scripts.Core.SaveLoad.Interfaces
{
    public interface ISaveable
    {
        string GetSaveId();
        JToken CaptureState();
        void RestoreState(JToken state);
    }
}