using SFML.Graphics;
using System.Collections.Generic;

namespace Shared
{
    public static class TextureManager
    {
        private static Dictionary<string, Texture> textureCache = new();
        public  static Dictionary<int , Texture> textures= new();

        public static void LoadTextures(){
            //idle 0-10
            textures[0]= GetTexture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_000.png");
            textures[1]=GetTexture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_001.png");
            textures[2]= GetTexture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_003.png");
            textures[3]=GetTexture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_004.png");
            textures[4]=GetTexture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_005.png");
            textures[5]=GetTexture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_006.png");
            textures[6]=GetTexture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_007.png");
            textures[7]=GetTexture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_008.png");
            textures[8]=GetTexture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_009.png");
            textures[9]=GetTexture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_010.png");
            textures[10]=GetTexture("src/assets/Player/01-Idle/PS_BALD GUY_Idle_011.png");
            //run 11-20 
            textures[11]=GetTexture("src/assets/Player/02-Run/PS_BALD GUY_Run_000.png");
            textures[12]=GetTexture("src/assets/Player/02-Run/PS_BALD GUY_Run_001.png");
            textures[13]=GetTexture("src/assets/Player/02-Run/PS_BALD GUY_Run_002.png");
            textures[14]=GetTexture("src/assets/Player/02-Run/PS_BALD GUY_Run_003.png");
            textures[15]=GetTexture("src/assets/Player/02-Run/PS_BALD GUY_Run_004.png");
            textures[16]=GetTexture("src/assets/Player/02-Run/PS_BALD GUY_Run_005.png");
            textures[17]=GetTexture("src/assets/Player/02-Run/PS_BALD GUY_Run_006.png");
            textures[18]=GetTexture("src/assets/Player/02-Run/PS_BALD GUY_Run_007.png");
            textures[19]=GetTexture("src/assets/Player/02-Run/PS_BALD GUY_Run_008.png");
            textures[20]=GetTexture("src/assets/Player/02-Run/PS_BALD GUY_Run_009.png");
            //jump 21-22
            textures[21] =GetTexture("src/assets/Player/04-Jump/PS_BALD GUY_JumpUp_000.png");
            textures[22] =GetTexture("src/assets/Player/04-Jump/PS_BALD GUY_JumpFall_000.png");
            //hurt 23-28
            textures[23] =GetTexture("src/assets/Player/06-Hurt/PS_BALD GUY_Hurt_000.png");
            textures[24] =GetTexture("src/assets/Player/06-Hurt/PS_BALD GUY_Hurt_001.png");
            textures[25] =GetTexture("src/assets/Player/06-Hurt/PS_BALD GUY_Hurt_002.png");
            textures[26] =GetTexture("src/assets/Player/06-Hurt/PS_BALD GUY_Hurt_003.png");
            textures[27] =GetTexture("src/assets/Player/06-Hurt/PS_BALD GUY_Hurt_004.png");
            textures[28] =GetTexture("src/assets/Player/06-Hurt/PS_BALD GUY_Hurt_005.png");
            //dead 29-38
            textures[29]=GetTexture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_000.png");
            textures[30]=GetTexture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_001.png");
            textures[31]=GetTexture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_002.png");
            textures[32]=GetTexture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_003.png");
            textures[33]=GetTexture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_004.png");
            textures[34]=GetTexture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_005.png");
            textures[35]=GetTexture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_006.png");
            textures[36]=GetTexture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_007.png");
            textures[37]=GetTexture("src/assets/Player/07-Dead/PS_BALD GUY_Dead_008.png");
            textures[38]=GetTexture("src/assets/Player/07-Dead/Dead10.png");

    }



        public static Texture GetTexture(string path)
        {
            if (!textureCache.ContainsKey(path))
            {
                textureCache[path] = new Texture(path);
            }
            return textureCache[path];
        }

        public static void ClearCache()
        {
            foreach (var texture in textureCache.Values)
            {
                texture.Dispose();
            }
            textureCache.Clear();
        }
    }
} 