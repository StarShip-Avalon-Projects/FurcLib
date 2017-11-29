using Furcadia.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Furcadia.Net.Web
{
    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class VariableIsConstantException : Exception
    {/// <summary>
     ///
     /// </summary>
        public VariableIsConstantException()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        public VariableIsConstantException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public VariableIsConstantException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected VariableIsConstantException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    ///
    /// </summary>
    public interface IVariable : IEquatable<IVariable>
    {/// <summary>
     ///
     /// </summary>
        string Name { get; }

        /// <summary>
        ///
        /// </summary>
        object Value { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public class Variable : IVariable
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
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
                int hashCode = (value != null ? value.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                return hashCode;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static readonly IVariable NoValue = new Variable("%none", null);

        private object value;

        /// <summary>
        ///
        /// </summary>
        public object Value
        {
            get { return value ?? "null"; }
            set
            {
                // removed Value = as it interfered with page.setVariable - Gerolkae
                if (!CheckType(value)) throw new TypeNotSupportedException(value.GetType().Name +
                " is not a supported type. Expecting string or double.");

                this.value = value;
            }
        }

        /// <summary>
        /// Name of the Variable
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Construct the Variable with name
        /// </summary>
        /// <param name="name"></param>
        public Variable(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Construct the variable with name and value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Variable(string name, object value)
        {
            Name = name;
            this.value = value;
        }

        private bool CheckType(object _value)
        {
            if (_value == null) return true;

            return _value is string ||
                   _value is double;
        }

        /// <summary>
        /// Returns a const identifier if the variable is constant followed by name,
        /// <para>otherwise just the name is returned.</para>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name} = {value ?? "null"}";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public Variable Clone()
        {
            return new Variable(Name, value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="varA"></param>
        /// <param name="varB"></param>
        /// <returns></returns>
        public static bool operator ==(Variable varA, Variable varB)
        {
            return varA.Value == varB.Value;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="varA"></param>
        /// <param name="varB"></param>
        /// <returns></returns>
        public static bool operator !=(Variable varA, Variable varB)
        {
            return varA.Value != varB.Value;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="varA"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static Variable operator +(Variable varA, double num)
        {
            varA.Value = varA.Value.As<double>() + num;
            return varA;
        }

        public static Variable operator -(Variable varA, double num)
        {
            varA.Value = varA.Value.As<double>() - num;
            return varA;
        }

        public static Variable operator *(Variable varA, double num)
        {
            varA.Value = varA.Value.As<double>() * num;
            return varA;
        }

        public static Variable operator /(Variable varA, double num)
        {
            varA.Value = varA.Value.As<double>() / num;
            return varA;
        }

        public static Variable operator +(Variable varA, string str)
        {
            varA.Value = varA.Value.As<string>() + str;
            return varA;
        }

        public static implicit operator string(Variable var)
        {
            return var.Value.As<string>();
        }

        public static implicit operator double(Variable var)
        {
            return var.Value.As<double>();
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is Variable && Equals((Variable)obj);
        }
    }

    /// <summary>
    ///
    /// </summary>
    [Serializable]
    public sealed class VariableList : IVariable
    {
        /// <summary>
        ///
        /// </summary>
        public string Name { get; private set; }

        private List<object> values;

        /// <summary>
        ///
        /// </summary>
        public object Value
        {
            get { return values.FirstOrDefault(); } // this is intended behavior just in case you try to get variable value without [index]
            set { if (!values.Contains(value)) values.Add(value); }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object this[int index]
        {
            get { return values[index]; }
        }

        /// <summary>
        ///
        /// </summary>
        public int Count { get => values.Count; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isConstant"></param>
        public VariableList(string name, bool isConstant = false)
        {
            Name = name;
            values = new List<object>();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        public VariableList(string name, params object[] values)
        {
            Name = name;
            this.values = new List<object>(values);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(IVariable other)
        {
            return Equals(values, other.Value) && string.Equals(Name, other.Name);
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
                int hashCode = (values != null ? values.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}