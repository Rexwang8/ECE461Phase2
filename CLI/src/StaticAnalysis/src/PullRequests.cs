namespace StaticAnalysis;
public static class PullRequests
{
    /*
    Responsive Maintainer


    FOR GITHUB
    Metric   Weight
    ------------------
    Issues                              50%
    Status                              30%
    Updates                             20%

    FOR NPM
    Metric   Weight
    ------------------
    Issues                              50%
    Status                              50%

    */
    public static float GetScore(URLInfo urlInfo)
    {   
        int totalChanges = urlInfo.githubTotalChanges;
        Console.WriteLine(totalChanges);
        Console.WriteLine("a hduiashdfioashdfiuoashdifaiosdhf uashodfuahsidofhaiusd fioas hdfiua hsuiofhasiudf hiaous hdfiuah sdfiua hsidfhauisd f");
        int prChanges = urlInfo.githubPRChanges;
        Console.WriteLine(prChanges);
        Console.WriteLine("kasdh fiuasdiof hausiodfh uaoishdfiua hsdifuh aiusdfh oiushdciuansdiufhaosd fhoiuashdfiu");
        double fraction = (double)prChanges / totalChanges;
        return (float)fraction;
    } 
}