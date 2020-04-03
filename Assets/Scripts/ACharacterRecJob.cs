using System;
using System.Collections.Generic;

public abstract class ACharacterRecJob : ICharacterJob
{
    protected readonly Queue<ICharacterJob> subJobs = new Queue<ICharacterJob>();
	public ACharacterRecJob()
	{
	}
    public virtual void DoProgress()
    {
        if (subJobs.Count == 0)
        {
            return;
        }
        ICharacterJob job = subJobs.Peek();
        if (job.IsDone())
        {
            subJobs.Dequeue();
            DoProgress();
            return;
        }
        job.DoProgress();
    }

    public abstract bool IsDone();
}
