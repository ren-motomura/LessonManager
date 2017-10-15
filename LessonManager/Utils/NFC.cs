using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace LessonManager.Utils
{
    static class NFC
    {
        [DllImport("OrangeOneStopEasyAPI.dll", EntryPoint = "GetCardID", CharSet = CharSet.Ansi)]
        private extern static int getCardID_(out Byte id, out int idLength);

        public enum Result
        {
            GET_ID_SUCCESS,
            GET_ID_FAILURE,
            GET_ID_NO_SERVICE,
            GET_ID_NO_READERS,
            GET_ID_NO_CARD,
            GET_ID_REMOVE_TIMEOUT,
            GET_ID_REMOVE_CARD,
            GET_ID_CARD_TIMEOUT,
            GET_ID_COMMAND_ERROR,
            GET_ID_RELEASE_ERROR,
        }

        public static Result GetCardID(out string idStr)
        {
            Byte[] id = new Byte[10];
            int idLength;
            int result = getCardID_(out id[0], out idLength);

            idStr = "";
            for (int i = 0; i < idLength; ++i)
            {
                idStr += id[i].ToString("X2");
            }

            return (Result)result;
        }
    }
}
