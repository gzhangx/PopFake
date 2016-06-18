using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Veda.Tests
{
    public class RandMock
    {
        object GetRandomEnumVal(Type etype)
        {


            Array values = Enum.GetValues(etype);
            Random random = new Random();
            return values.GetValue(random.Next(values.Length));
        }



        class Setter
        {
            public PropertyInfo Prop { get; set; }
            public FieldInfo Field { get; set; }
            public void SetValue(object instance, object val)
            {
                try
                {
                    if (Prop != null) Prop.SetValue(instance, val, null);
                    if (Field != null) Field.SetValue(instance, val);
                } catch (Exception exc)
                {
                    Console.WriteLine("error setting intance " + instance.GetType());
                }
            }
            public Type PropertyType
            {
                get
                {
                    if (Prop != null) return Prop.PropertyType;
                    return Field.FieldType;
                }
            }

            public string Name
            {
                get
                {
                    if (Prop != null) return Prop.Name;
                    return Field.Name;
                }
            }
            public override string ToString()
            {
                return "Setter " + (Prop != null ? (Prop.Name + " " + Prop.PropertyType) : "") + (Field != null ? (Field.Name + " " + Field.FieldType) : "");
            }
        }
        static string sampleText = "aa bb cc dd ee ff gg hh ii jj kk ll mm nn oo pp qq rr ss tt";
        Random random = new Random();
        public int DynaCast(Type type, int input)
        {
            FieldInfo maxValueField = type.GetField("MaxValue", BindingFlags.Public
        | BindingFlags.Static);
            int max = 1000000;
            if (Int32.TryParse(maxValueField.GetValue(null).ToString(), out max))
            {
                if (input > max && max != 0) input = input % max;
            }
            return input;
        }
        int startPos = 1000;
        private object GenerateValueForType(Type pPropertyType, string name, int level, ParentInfo pinf)
        {
            startPos++;
            {
                var PropertyType = Nullable.GetUnderlyingType(pPropertyType) ?? pPropertyType;
                if (PropertyType.IsEnum)
                {
                    return (GetRandomEnumVal(PropertyType));
                }
                else if (PropertyType == typeof(string))
                {
                    //var strv = sampleText.Substring(0, random.Next(1, sampleText.Length - 1)).Trim();

                    //return (strv);
                    return name + "_" + startPos;
                }
                else if (PropertyType == typeof(DateTime))
                {
                    var date = DateTime.Now.AddDays(random.Next(1000));
                    return (date);
                }
                else if (PropertyType == typeof(DateTimeOffset))
                {
                    var date = DateTime.Now.AddDays(random.Next(1000));
                    return (date);
                }
                else if (PropertyType == typeof(TimeSpan))
                {
                    var span = TimeSpan.FromMilliseconds(startPos);
                    return span;
                }
                else if (PropertyType == typeof(Guid))
                {
                    return (Guid.NewGuid());
                }
                else if (PropertyType == typeof(decimal))
                {
                    if (pPropertyType == typeof(decimal))
                        return ((decimal?)startPos / 100.0m);
                    else
                        return (startPos / 100.0m);
                }
                else if (PropertyType.IsPrimitive)
                {
                    
                    //if (PropertyType.IsPrimitive)
                    {
                        bool isNullable = false;
                        try
                        {
                            isNullable = pPropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
                        }
                        catch { };
                        if (PropertyType == typeof(char))
                        {
                            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                            return (chars[startPos % chars.Length]);
                        }
                        else if (isNullable)
                        {
                            if (PropertyType == typeof(long)) return ((long?)startPos);
                            else if (PropertyType == typeof(int)) return ((int?)startPos);
                            else if (PropertyType == typeof(decimal)) return ((decimal?)startPos);
                            else if (PropertyType == typeof(short)) return ((short?)startPos);
                            else if (PropertyType == typeof(uint)) return ((uint?)startPos);
                            else if (PropertyType == typeof(ushort)) return ((ushort?)startPos);
                            else if (PropertyType == typeof(byte)) return ((byte?)startPos);
                            else if (PropertyType == typeof(char)) return ((char?)startPos);
                            else if (PropertyType == typeof(long)) return ((long?)startPos);
                            else if (PropertyType == typeof(double)) return ((double?)startPos);
                            else if (PropertyType == typeof(bool)) return ((bool?)(startPos % 2 == 0));
                            else throw new Exception("not handled for type " + PropertyType);
                        }
                        else
                        {
                            if (PropertyType == typeof(bool))
                            {
                                return ((bool)(startPos % 2 == 0));
                            }
                            else
                                return (DynaCast(PropertyType, startPos));
                        }

                    }

                }
                else if (PropertyType.IsArray)
                {


                    if (PropertyType == typeof(String[]))
                    {
                        EmitMark("new []{");
                        string[] res = new string[random.Next(10) + 1];
                        for (int i = 0; i < res.Length; i++)
                        {
                            res[i] = name + "_" + startPos++;  //sampleText.Substring(0, random.Next(1, sampleText.Length - 1)).Trim();
                            EmiteValue(typeof(string), res[i]);
                        }

                        EmitMark(" ".PadLeft(level) + "},\r\n");

                        return (res);
                    }
                    else
                    {
                        EmitMark("null,\r\n");
                        Console.WriteLine("not implemented " + PropertyType);
                        return null;
                    }
                    
                }
                else
                {
                    {
                        if (PropertyType.IsAbstract || PropertyType.IsInterface)
                        {
                            EmitMark("null,\r\n");
                            return null;
                        }
                        object subfield = null;
                        try
                        {
                            subfield = Activator.CreateInstance(PropertyType);
                        } catch (Exception exc)
                        {
                            Console.WriteLine("WARNING: Can't create " + PropertyType + " " + exc.Message);
                        }
                        if (PropertyType.Name == "List`1")
                        {
                            var t = PropertyType.GetGenericArguments()[0];
                            if (pinf.ParentIgnores.Any(tt=>tt ==t))
                            {
                                Console.WriteLine("type is recurisive, ignored " + t);
                                EmitMark("null,\r\n");
                                return null;
                            }
                            //pinf.ParentIgnores.Add(t);
                            EmitMark(" ".PadLeft(level)+"new List<" + t.FullName + ">{\r\n");
                            for (int i = 0; i < random.Next(10) + 1; i++)
                            {
                                var pi = new List<Type>();
                                if (pinf != null && pinf.ParentIgnores != null) pi.AddRange(pinf.ParentIgnores);
                                var listVal = GenerateValueForType(t, name, level + 1, new ParentInfo
                                {
                                    instanceType = t,
                                    ParentIgnores = pi,
                                });
                                if (subfield != null)
                                subfield.GetType().GetMethod("Add").Invoke(subfield, new[] { listVal });
                                if (t.IsPrimitive)
                                {
                                    EmitMark(" ".PadLeft(level)); EmiteValue(t, listVal);
                                }
                            }
                            EmitMark("},\r\n");
                        }
                        else if (PropertyType.Name == "Dictionary`2")
                        {
                            var ts = PropertyType.GetGenericArguments();
                            EmitMark(" ".PadLeft(level) + "new Dictionary<" + ts[0].Name + ","  + ts[1].Name +">{\r\n");
                            var pi = new List<Type>();
                            if (pinf != null && pinf.ParentIgnores != null) pi.AddRange(pinf.ParentIgnores);
                            for (int i = 0; i < 3; i++)
                            {
                                startPos++;
                                var key = GenerateValueForType(ts[0], name, level + 1, new ParentInfo { ParentIgnores = pi });
                                var val = GenerateValueForType(ts[1], name, level + 1, new ParentInfo { ParentIgnores = pi });
                                EmitMark(" ".PadLeft(level) + " {");
                                EmiteValue(ts[0], key);
                                EmiteValue(ts[1], key, "");
                                EmitMark(" ".PadLeft(level) +"},\r\n");
                            }
                            EmitMark(" ".PadLeft(level) + "},\r\n");
                        }
                        else
                        //var subfield = new Hydrator<object>(PropertyType, null).GetSingle();// Activator.CreateInstance(PropertyType);
                        {
                            if (pinf.ParentIgnores.Any(tt => tt == PropertyType))
                            {
                                Console.WriteLine("type is recurisive, ignored =>" + PropertyType);
                                EmitMark("null,\r\n");
                                return null;
                            }
                            pinf.ParentIgnores.Add(PropertyType);
                            var pi = new List<Type>();
                            if (pinf != null && pinf.ParentIgnores != null) pi.AddRange(pinf.ParentIgnores);
                            EmitMark(" ".PadLeft(level) + "new " + PropertyType.FullName+"{\r\n");
                            Populate(subfield, level + 1, new ParentInfo { instanceType = PropertyType, ParentIgnores = pi });
                            EmitMark(" ".PadLeft(level) + "},\r\n");
                        }
                        return (subfield);
                    }

                }
            }
        }

        class ParentInfo
        {
            public Type instanceType { get; set; }
            public List<Type> ParentIgnores { get; set; }
        }
        private void Populate(object instance, int level, ParentInfo pinf)
        {
            var t = pinf == null ? null: pinf.instanceType;
            if (t == null) t = instance.GetType();
            //AddTypeMapToPropertyMap();
            //foreach (IMapping mapping in propertyMap.Values)
            //{
            //    PropertyInfo propertyInfo = instance.GetType().GetProperty(mapping.PropertyName, BindingFlags.Public | BindingFlags.Instance);


            //    if (propertyInfo != null)
            //    {
            //        propertyInfo.SetValue(instance, mapping.Generate(), null);
            //    }
            //}

            List<Setter> setters = new List<Setter>();
            foreach (var p in t.GetProperties().Where(p => p.CanWrite)) setters.Add(new Setter { Prop = p });
            foreach (var p in t.GetFields()) setters.Add(new Setter { Field = p });
            foreach (var p in setters)
            {
                EmiteProperty(level, p.Name);
                var pi = new List<Type>();
                if (pinf != null && pinf.ParentIgnores != null) pi.AddRange(pinf.ParentIgnores);
                //pi.Add(p.PropertyType);
                var res = GenerateValueForType(p.PropertyType, p.Name, level + 1, new ParentInfo
                {
                    ParentIgnores = pi
                });
                EmiteValue(p.PropertyType, res);
                if (instance != null)
                    p.SetValue(instance, res);
            }
        }

        public interface SWriter
        {
            void Write(object s);
            void WriteLine(string s);
        }
        SWriter sw;
        private void EmiteProperty(int level, string name)
        {
            if (sw == null) return;
            sw.Write(string.Format("{0}{1} = ", " ".PadLeft(level), name));
        }
        private void EmitMark(string mark)
        {
            if (sw == null) return;
            sw.Write(mark);
        }
        private void EmiteValue(Type type, object val, string ending = ",")
        {
            if (sw == null) return;
            var PropertyType = Nullable.GetUnderlyingType(type) ?? type;
            
            if (PropertyType.IsEnum)
            {
                sw.Write(val.GetType().FullName + "."+val);
            }
            else if (PropertyType == typeof(string))
            {
                sw.Write("\"" + val + "\"");
            }
            else if (PropertyType == typeof(DateTime))
            {
                sw.Write("DateTime.Parse(\"" + val+"\")");
            }
            else if (PropertyType == typeof(DateTimeOffset))
            {
                sw.Write("DateTime.Parse(\"" + val + "\")");
            }
            else if (PropertyType == typeof(TimeSpan))
            {
                sw.Write("TimeSpan.Parse(\"" + val + "\")");
            }
            else if (PropertyType == typeof(Guid))
            {
                sw.Write("Guid.Parse(\"" + val + "\")");
            }
            else if (PropertyType == typeof(decimal))
            {
                sw.Write(val+"m");
            }
            else if (PropertyType.IsPrimitive)
            {
                if (PropertyType == typeof(bool))
                {
                    sw.Write((bool)val ? "true" : "false");
                }else
                sw.Write(val);
            }
            else if (PropertyType.IsArray)
            {
                return;
            }
            else
            {
                return;

            }
            //sw.WriteLine(", //" +type + " " + val);
            sw.WriteLine(ending);
        }
        //
        public T Generate<T>(Action<object> init = null, SWriter sw = null)
        {
            return (T)GenerateByType(typeof(T), o =>
            {
                if (init != null) init((T)o);
            }, sw);
        }
        public object GenerateByType(Type t, Action<object> init = null, SWriter sw = null)
        {
            this.sw = sw;
            {
                var instance = Activator.CreateInstance(t);
                if (init != null) init(instance);
                Populate(instance, 0, new ParentInfo
                {
                    ParentIgnores = new List<Type>
                    {
                        t
                    }
                });
                return instance;
            }
        }
    }
}
