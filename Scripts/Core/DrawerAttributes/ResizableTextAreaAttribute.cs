using System;

namespace NaughtyAttributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class ResizableTextAreaAttribute : DrawerAttribute
	{
        private readonly int minimumLines;
        private readonly int maximumLines;

        public ResizableTextAreaAttribute(int minimumLines = 1, int maximumLines = 0)
		{
            this.minimumLines = minimumLines;
            this.maximumLines = maximumLines;
        }

        public int MinimumLines => minimumLines;

        public int MaximumLines => maximumLines;
    }
}
