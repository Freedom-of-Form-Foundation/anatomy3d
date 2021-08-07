using System;
using System.Collections.Generic;

namespace FreedomOfFormFoundation.AnatomyEngine.Calculus
{
    /// <summary>
    /// MovingPoint2D represents a point that is expected to move around, usually due to user interaction.
    /// If a MovingPoint2D moves, it notifies a callback, which will usually be a collection of points that may need
    /// to recalculate some values when points change. MovingPoint2D is not threadsafe; it assumes that it will be
    /// manipulated by a UI thread and any reads to the collection it is a member of will be synchronized to that
    /// thread in some way.
    ///
    /// MovingPoint2D's sort order is to sort by X, breaking ties by Y. NaN is sorted according to Microsoft's
    /// CompareTo implementation which, unlike operators, has a defined sort order for NaN.
    /// </summary>
    public class MovingPoint2D: IComparable<MovingPoint2D>,IEquatable<MovingPoint2D>
    {
        private Real _x, _y;
        private Action _changed;

        public Real X
        {
            get => _x;
            set
            {
                if (_x == value) return;
                _x = value;
                _changed();
            }
        }

        public Real Y
        {
            get => _y;
            set
            {
                if (_y == value) return;
                _y = value;
                _changed();
            }
        }

        /// <summary>
        /// Constructs a MovingPoint with a given update callback and starting position.
        /// </summary>
        /// <param name="changeCallback">Invoked when the point moves.</param>
        /// <param name="x">Starting X position.</param>
        /// <param name="y">Starting Y position.</param>
        public MovingPoint2D(Action changeCallback, Real x, Real y)
        {
            _x = x;
            _y = y;
            _changed = changeCallback;
        }

        /// <summary>
        /// Constructs a MovingPoint with a given update callback at 0, 0.
        /// </summary>
        /// <param name="changeCallback">Invoked when the point moves.</param>
        public MovingPoint2D(Action changeCallback) : this(changeCallback, 0, 0)
        {
        }

        /// <inheritdoc/>
        public int CompareTo(MovingPoint2D other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            int xComparison = _x.CompareTo(other._x);
            if (xComparison != 0) return xComparison;
            return _y.CompareTo(other._y);
        }

        public static bool operator <(MovingPoint2D left, MovingPoint2D right)
        {
            return Comparer<MovingPoint2D>.Default.Compare(left, right) < 0;
        }

        public static bool operator >(MovingPoint2D left, MovingPoint2D right)
        {
            return Comparer<MovingPoint2D>.Default.Compare(left, right) > 0;
        }

        public static bool operator <=(MovingPoint2D left, MovingPoint2D right)
        {
            return Comparer<MovingPoint2D>.Default.Compare(left, right) <= 0;
        }

        public static bool operator >=(MovingPoint2D left, MovingPoint2D right)
        {
            return Comparer<MovingPoint2D>.Default.Compare(left, right) >= 0;
        }

        public bool Equals(MovingPoint2D other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _x.Equals(other._x) && _y.Equals(other._y);
        }

        public static bool operator ==(MovingPoint2D left, MovingPoint2D right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MovingPoint2D left, MovingPoint2D right)
        {
            return !Equals(left, right);
        }
    }
}
