using System.Reflection;

namespace SimpleTesting.TestAdapter
{
    internal class Test
    {
        public string File { get; private set; }

        public int Line { get; private set; }

        public string FullyQualifiedName { get; private set; }

        public string DisplayName { get; private set; }

        public MethodInfo Method { get; }

        public bool IsSkipped { get; private set; }

        public Test(MethodInfo method)
        {
            this.Method = method;

            this.SetupSkipped();
            this.SetupName();
            this.SetupLocation();
        }

        public override string ToString() => this.Method.ToString();

        private void SetupSkipped()
        {
            var attribute = this.Method.GetCustomAttribute<SkipAttribute>();
            if (attribute != null)
            {
                this.IsSkipped = true;
                this.UpdateLine(attribute.Line);
            }
        }

        private void SetupName()
        {
            this.DisplayName = $"{this.Method.DeclaringType.Name}.{this.Method.Name}";
            this.FullyQualifiedName = $"{this.Method.DeclaringType.FullName}.{this.Method.Name}";
        }

        private void SetupLocation()
        {
            var attribute = this.Method.GetCustomAttribute<FactAttribute>();
            this.File = attribute.File;
            this.UpdateLine(attribute.Line);
        }

        ///checking line on each attribute to find best match.
        ///generally, next string will be the one with method
        private void UpdateLine(int line) => this.Line = (line + 1) > this.Line ? (line + 1) : this.Line;
    }
}