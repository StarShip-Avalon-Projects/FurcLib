using Furcadia.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Furcadia.Net.Web
{
    [Serializable]
    public class VariableIsConstantException : Exception
    {
        public VariableIsConstantException()
        {
        }

        public VariableIsConstantException(string message)
            : base(message)
        {
        }

        public VariableIsConstantException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected VariableIsConstantException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    public class VariableEqualityComparer : IEqualityComparer<IVariable>
    {
        public bool Equals(IVariable x, IVariable y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(IVariable obj)
        {
            return obj.GetHashCode();
        }
    }

    public interface IVariable : IEquatable<IVariable>
    {
        string Name { get; }
        object Value { get; set; }
        bool IsConstant { get; }
    }

    [Serializable]
    [CLSCompliant(false)]
    public class Variable : IVariable
    {
        public bool Equals(IVariable other)
        {
            return Equals(value, other.Value) && string.Equals(Name, other.Name);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return Name.GetHashCode();
            }
        }

        public static readonly IVariable NoValue = new Variable("%none", null, true);

        public bool IsConstant
        {
            get;
            private set;
        }

        private object value;

        public object Value
        {
            get { return value; }
            set
            {
                // removed Value = as it interfered with page.setVariable - Gerolkae
                if (!CheckType(value)) throw new TypeNotSupportedException(value.GetType().Name +
                " is not a supported type. Expecting string, double or variable.");

                if (value != null && IsConstant)
                    throw new VariableIsConstantException($"Attempt to assign a value to constant '{Name}'");
                if (value is IVariable)
                    this.value = (value as IVariable).Value;
                else
                    this.value = value;
            }
        }

        public string Name { get; internal set; }

        internal Variable(string name, bool constant = false)
        {
            IsConstant = constant;
            Name = name;
        }

        internal Variable(string name, object value, bool constant = false)
        {
            IsConstant = constant;
            Name = name;
            this.value = value;
        }

        private bool CheckType(object _value)
        {
            if (_value == null) return true;

            return _value is string ||
                   _value is double ||
                   _value is IVariable;
        }

        /// <summary>
        /// Returns a const identifier if the variable is constant followed by name,
        /// <para>otherwise just the name is returned.</para>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ((IsConstant) ? "const " : "") + $"{Name} = {value ?? "null"}";
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="asConstant">Clone as Constant</param>
        /// <returns></returns>
        public Variable Clone(bool asConstant = false)
        {
            return new Variable(Name, value, asConstant);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="varA">The variable a.</param>
        /// <param name="varB">The variable b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Variable varA, Variable varB)
        {
            return varA.Value == varB.Value;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="varA">The variable a.</param>
        /// <param name="varB">The variable b.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Variable varA, Variable varB)
        {
            return varA.Value != varB.Value;
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="varA">The variable a.</param>
        /// <param name="num">The number.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Variable operator +(Variable varA, double num)
        {
            varA.Value = varA.Value.AsDouble() + num;
            return varA;
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="varA">The variable a.</param>
        /// <param name="num">The number.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Variable operator -(Variable varA, double num)
        {
            varA.Value = varA.Value.AsDouble() - num;
            return varA;
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="varA">The variable a.</param>
        /// <param name="num">The number.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Variable operator *(Variable varA, double num)
        {
            varA.Value = varA.Value.AsDouble() * num;
            return varA;
        }

        /// <summary>
        /// Implements the operator /.
        /// </summary>
        /// <param name="varA">The variable a.</param>
        /// <param name="num">The number.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Variable operator /(Variable varA, double num)
        {
            varA.Value = varA.Value.AsDouble() / num;
            return varA;
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="varA">The variable a.</param>
        /// <param name="str">The string.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Variable operator +(Variable varA, string str)
        {
            varA.Value = varA.Value.AsDouble() + str;
            return varA;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Variable"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="var">The variable.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator string(Variable var)
        {
            return var.Value.AsString();
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Variable"/> to <see cref="System.Double"/>.
        /// </summary>
        /// <param name="var">The variable.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator double(Variable var)
        {
            return var.Value.AsDouble();
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is Variable && Equals((Variable)obj);
        }
    }
}