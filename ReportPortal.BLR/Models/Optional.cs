namespace ReportPortal.BL.Models
{
    public sealed class Optional<T>
    {
        public bool HasValue { get; }
        public T Value { get; }

        private Optional() => HasValue = false;

        public Optional(T value)
        {
            Value = value;
            HasValue = true;
        }

        public static Optional<T> None() => new Optional<T>();
    }
}
