using Furcadia.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Furcadia.Net.Web
{
    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class VariableIsConstantException : Exception
    {
        /// <summary>
        ///
        /// </summary>
        public VariableIsConstantException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableIsConstantException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public VariableIsConstantException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableIsConstantException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public VariableIsConstantException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableIsConstantException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected VariableIsConstantException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    ///
    /// </summary>
    public class VariableEqualityComparer : IEqualityComparer<IVariable>
    {
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        public bool Equals(IVariable x, IVariable y)
        {
            return x.Equals(y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public int GetHashCode(IVariable obj)
        {
            return obj.GetHashCode();
        }
    }

    /// <summary>
    ///
    /// </summary>
    public interface IVariable : IEquatable<IVariable>
    {
        /// <summary>
        ///
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        object Value { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is constant.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is constant; otherwise, <c>false</c>.
        /// </value>
        bool IsConstant { get; }
    }

    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="Furcadia.Net.Web.IVariable" />
    [Serializable]
    [CLSCompliant(false)]
    public class Variable : IVariable
    {
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
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

        /// <summary>
        /// The no value
        /// </summary>
        public static readonly IVariable NoValue = new Variable("%none", null);

        /// <summary>
        /// Gets a value indicating whether this instance is constant.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is constant; otherwise, <c>false</c>.
        /// </value>
        public bool IsConstant
        {
            get => false;
        }

        /// <summary>
        /// The value
        /// </summary>
        private object value;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        /// <exception cref="TypeNotSupportedException"></exception>
        /// <exception cref="Furcadia.Net.Web.VariableIsConstantException"></exception>
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

        /// <summary>
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Variable"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public Variable(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Variable"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public Variable(string name, object value)
        {
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
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public Variable Clone()
        {
            return new Variable(Name, value);
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

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj != null && obj is Variable && Equals((Variable)obj);
        }
    }
}