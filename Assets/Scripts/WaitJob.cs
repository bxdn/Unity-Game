using System;

public class WaitJob : ICharacterJob
{
    private int timeWaited = 0;
    private readonly int waitThresh;
	public WaitJob(int waitThresh)
	{
        this.waitThresh = waitThresh;
	}

    public void DoProgress()
    {
        timeWaited++;
    }

    public bool IsDone()
    {
        return timeWaited == waitThresh;
    }
}
