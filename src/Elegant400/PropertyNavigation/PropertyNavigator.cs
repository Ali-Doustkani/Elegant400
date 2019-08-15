using Elegant400.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Elegant400.PropertyNavigation
{
   /// <summary>
   /// Navigates through all properties of parent object and it's children objects.
   /// </summary>
   public class PropertyNavigator
   {
      public PropertyNavigator(object model)
      {
         _path = new Stack<object>();
         _iterator = new IteratorStack();
         _iterator.PushObject(model);
      }

      private readonly IteratorStack _iterator;
      private readonly Stack<object> _path;
      private int _index;

      public Property Property { get; private set; }

      public void ForEachAttribute(Action<ValidationAttribute, Property> action)
      {
         foreach (var attrib in Property.Attributes)
            action(attrib, Property);
      }

      public bool Read()
      {
         if (!_iterator.Any())
            return false;

         if (_iterator.MoveNext())
         {
            if (HasAttribute())
            {
               MakeProperty();
               StepInIfClass();
               return true;
            }
            StepInIfClass();
            return Read();
         }
         else
         {
            StepOut();
            return Read();
         }
      }

      private bool HasAttribute() =>
        _iterator.Current is PropertyInfo && _iterator.CurrentProperty.GetCustomAttributes().Any();

      private void MakeProperty()
      {
         var path = _path.Reverse().Concat(new[] { _iterator.CurrentProperty.Name.ToCamelCase() }).ToArray();
         var attributes = _iterator.CurrentProperty
            .GetCustomAttributes()
            .OfType<ValidationAttribute>()
            .Cast<ValidationAttribute>();
         Property = new Property(attributes, _iterator.CurrentProperty.GetValue(_iterator.CurrentOwner), path);
      }

      private void StepInIfClass()
      {
         if (_iterator.Current is PropertyInfo &&
            (_iterator.CurrentProperty.PropertyType == typeof(string) || _iterator.CurrentProperty.PropertyType.IsPrimitive))
            return;

         if (IsCollection())
         {
            _path.Push(_iterator.CurrentProperty.Name.ToCamelCase());
            _iterator.PushCurrentAsCollection();
         }
         else if (_iterator.Current is PropertyInfo)
         {
            _path.Push(_iterator.CurrentProperty.Name.ToCamelCase());
            _iterator.PushCurrentAsObject();
         }
         else
         {
            _path.Push(_index);
            _index++;
            _iterator.PushObject(_iterator.Current);
         }
      }

      private bool IsCollection() =>
        _iterator.Current is PropertyInfo && typeof(IEnumerable).IsAssignableFrom(_iterator.CurrentProperty.PropertyType);

      private void StepOut()
      {
         _iterator.Pop();
         if (_path.Any())
            _path.Pop();
      }
   }
}
