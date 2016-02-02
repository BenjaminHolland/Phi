using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phi.IO.Parsing {

    public static class HexParser {

        private static readonly byte[] _toByteArrayLookup = new byte[]{
                                                                     0x00,0x01,0x02,0x03,0x04,0x05,0x06,0x07,0x08,0x09,
                                                                     0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
                                                                     0x0A,0x0B,0x0C,0x0D,0x0E,0x0F
        };
        private static readonly uint[] _toHexStringLookup=_createToHexLookup();
        
        private static uint[] _createToHexLookup(){
            var result=new uint[256];
            for(int i=0;i<256;i++){
                string s=i.ToString("X2");
                result[i]=((uint)s[0])+((uint)s[1]<<16);
            }
            return result;
        }
        public static byte[] ToByteArray(string hexString) {
            if(hexString.Length<2){
                throw new ArgumentException("hexString Length");
            }
            if (hexString.Length % 2 != 0) {
                throw new ArgumentException("hexString Length");
            }
            if (hexString == null) {
                throw new ArgumentNullException("hexString");
            }
            byte[] ret = new byte[hexString.Length / 2];
            try {
                int cIdx = 0;
                for (int i = 0; i < hexString.Length/2; i ++,cIdx+=2) {
                    
                    checked {
                        ret[i] = (byte)((_toByteArrayLookup[hexString[cIdx] - 48])<<4 | (_toByteArrayLookup[hexString[cIdx + 1] - 48]));
                    }
                   
                }
            }
            catch (OverflowException) {
                throw new ArgumentException("hexString");
            }
            catch (IndexOutOfRangeException) {
                throw new ArgumentException("hexString");
            }

            return ret;
        }
        private static string ToHexString(byte[] data) {
            var result = new char[data.Length * 2];
            for (int i = 0; i < data.Length; i++) {
                var val = _toHexStringLookup[data[i]];
                result[2*i]=(char)val;
                result[2 * i + 1] = (char)(val >> 16);
            }
            return new string(result);
        }
    }
}
