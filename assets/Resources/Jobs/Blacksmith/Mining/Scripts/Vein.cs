using UnityEngine;

public class Vein : Item
{
    public Ore Ore;
    public Gem Gem;
    private int maxOres;
    public int ores;
    public int oreRarity;
    private float oneOre = 2f;
    private float delayTime = 1f;
    private int di = 1;
    private float prev = 0f;
    private float time = 0;
    public bool pure = false;
    [Range(0.5f, .98f)]
    public float high;
    public bool gem;
    [Range(0f, 0.8f)]
    public float low;
    public GameObject vein;
    public Geode geode;
    void Start()
    {
        float s = Random.Range(0f, 1f);
        if(!gem)
        {
            if (s > high)
            {
                maxOres = 6;
                ores = 6;
            }
            else if (s < low)
            {
                maxOres = 4;
                ores = 4;
            }
            else
            {
                maxOres = 5;
                ores = 5;
            }
        }
        else
        {
            if(pure)
            {
                if (Random.Range(0f, 1f) > .8f)
                {
                    maxOres = 2;
                    ores = 2;
                }
                else
                {
                    maxOres = 1;
                    ores = 1;
                }
            }
            else
            {
                maxOres = 5;
                ores = 5;
            }
        }
        setChar();
    }

    public override void Interact()
    {
        if(getChar().getCurrentInvSize() < getChar().getMaxInvSize())
        {
            if (ores > 0)
            {
                startMining();
            }
        }
    }

    private void startMining()
    {
        getCamera().setBusy(true);
        getChar().setControl(false);
        getCamera().openMining();
        mining = true;
        getCamera().updateBar(0f);
    }

    private bool mining = false;
    private bool delaying = false;
    void Update()
    {
        if (mining)
        {
            if(ores > 0)
            {
                if(!delaying)
                {
                    time += Time.deltaTime;
                    if (time >= oneOre)
                    {
                        if (pure)
                        {
                            getChar().minePure(this);
                            time = 0f;
                            ores--;
                            Delay();
                        }
                        else {
                            getChar().mine(this);
                            time = 0f;
                            ores--;
                            Delay();
                        }
                    }
                }
                else
                {
                    time += Time.deltaTime;
                    updateBar();
                    if (time >= delayTime)
                    {
                        di++;
                        prev += (time / delayTime) / maxOres;
                        delaying = false;
                        time = 0;
                    }
                }
            }
            else if(delaying)
            {
                if (delaying)
                {
                    time += Time.deltaTime;
                    updateBar();
                    if (time >= delayTime)
                    {
                        di++;
                        prev += (time / delayTime) / maxOres;
                        delaying = false;
                        stopMining();
                        time = 0;
                        stopMining();
                    }
                }
            }
            else
            {
                Delay();
            }
        }
    }

    private void stopMining()
    {
        mining = false;
        if (vein != null)
        {
            vein.SetActive(false);
        }
        getChar().setControl(true);
        getCamera().setBusy(false);
        getCamera().closeMining();
    }

    private void Delay()
    {
        delaying = true;
    }

    private void updateBar()
    {
        getCamera().updateBar(prev + ((time / delayTime) / maxOres));
    }
    public override GameObject getObject()
    {
        return null;
    }
    public override void set(Item i)
    {

    }
}
