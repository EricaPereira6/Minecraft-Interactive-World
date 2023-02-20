using UnityEngine;

public class Utils
{

    static float smooth = 0.00055f;
    static int maxDirtHeight = 150;
    static int octaves = 8;
    static float persistence = 0.87f;
    static float offset = 32000f;  // funciona como uma seed


    static float stoneSmooth = smooth * 5f;
    static float maxStoneOffset = maxDirtHeight - 12;
    static int stoneOctaves = octaves - 3;
    static float stonePersistence = persistence * 1.2f;

    static float smooth3D = smooth * 3f;


    public static int chunkSize = 16;
    public static int radius = 3;
    public static string untouchableString = "U ";

    public static float caveFBMValue = 0.51f;
    public static int caveOctaves = 5;
    public static float cavePersistence = 1.5f;

    public static int   diamondHeightOffset = 15;
    public static float diamondFBMValue = 0.44f;
    public static float diamondFBMOffset = 0.00005f;
    public static int   diamondOctaves = 11;
    public static float diamondPersistence = 1.5f;
    public static float minDiamondOffset = 0.005f;
    public static float maxDiamondOffset = minDiamondOffset + 0.02f;

    public static float granitePercentage = 0.2f;

    public static float coalPercentage = 0.1f;
    public static float coalFBMValue = 0.425f;
    public static float coalOffset = 0.008f;

    public static float grassFBMValue = 0.451f;
    public static float grassFBMOffset = 0.015f;
    public static int   grassOctaves = 10;
    public static float grassPersistence = 1.2f;

    public static float flowerFBMOffset = grassFBMOffset / 15f;


    public static int GenerateHeight(float x, float z)
    {
        return (int)Map(0, maxDirtHeight, 0, 1, FBM(x * smooth, z * smooth, octaves, persistence));
    }

    public static int GenerateStoneHeight(float x, float z)
    {
        return (int)Map(0, maxStoneOffset, 0, 1, FBM(x * stoneSmooth, z * stoneSmooth, stoneOctaves, stonePersistence));
    }

    // transforma-se a funcao entre [0, 1] para [min, max] para ter um terreno com dimensoes mais adequadas
    // inverseLerp retorna o meu t na funcao :  val = (1 - t) orimin + torimax
    static float Map(float newmin, float newmax, float orimin, float orimax, float val)
    {
        return Mathf.Lerp(newmin, newmax, Mathf.InverseLerp(orimin, orimax, val));
    }


    //fazer a media para que haja coerencia de valores de ruido de perlin em todas as direcoes
    // dentro de contexto, permite fazer grutas/buracos
    // retorna valores entre [0, 1] que serao interpretados como uma probabilidade
    public static float FBM3D(float x, float y, float z, int octaves, float persistence)
    {
        float xy = FBM(x * smooth3D, y * smooth3D, octaves, persistence);
        float yx = FBM(y * smooth3D, x * smooth3D, octaves, persistence);
        float xz = FBM(x * smooth3D, z * smooth3D, octaves, persistence);
        float zx = FBM(z * smooth3D, x * smooth3D, octaves, persistence);
        float zy = FBM(z * smooth3D, y * smooth3D, octaves, persistence);
        float yz = FBM(y * smooth3D, z * smooth3D, octaves, persistence);

        return (xy + yx + xz + zx + zy + yz) / 6;
    }


    // n Octaves - o numero de componentes de freq
    // a medida que a frequencia vai aumentando, a persistencia vai diminuindo a amplitude
    // menor persistence -> as octaves de alta frequencia tem menos importancia (resulta em mais suavidade)
    public static float FBM(float x, float z, int nOctaves, float persistence)
    {

        float total = 0;
        float amplitude = 1;
        float frequency = 1;
        float maxValue = 0;

        for (int i = 0; i < nOctaves; i++)
        {
            //soma de varias funcoes em entrevalos entre [0,1]
            total += Mathf.PerlinNoise((x + offset) * frequency, (z + offset) * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= 2;

        }

        // normalizar a soma das varias funcoes
        return total / maxValue;
    }


    /*
    // Update is called once per frame
    void Update()
    {
        t += inc;
        float n = FBM(t, 4, 0.6f);
        Grapher.Log(n, "fBM", Color.yellow);
    }
    */
}
