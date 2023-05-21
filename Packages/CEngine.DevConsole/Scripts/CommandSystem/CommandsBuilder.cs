using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CYM.DevConsole.Command
{
	public class CommandsBuilder
	{
		public class MemberBuilderSettings
		{
			private List<string> nameExceptionsList = new List<string>();

			private List<MemberInfo> memberExceptionsList = new List<MemberInfo>();

			public StaticBindings staticBindings { get; private set; }

			public AccesModifierBindings accesModiferBindings { get; set; }

			public string[] nameExceptions { get; private set; }

			public MemberInfo[] memberExceptions { get; private set; }

			public bool includeObsolete { get; set; }

			public MemberBuilderSettings()
			{
				staticBindings = StaticBindings.Static;
				accesModiferBindings = AccesModifierBindings.Public;
				nameExceptions = new string[0];
				memberExceptions = new MemberInfo[0];
			}

			public void AddExceptions(params string[] names)
			{
				nameExceptionsList.AddRange(names);
				nameExceptions = nameExceptionsList.ToArray();
			}

			public void AddExceptions(params MemberInfo[] members)
			{
				memberExceptionsList.AddRange(members);
				memberExceptions = memberExceptionsList.ToArray();
			}

			public bool IsException(MemberInfo member)
			{
				for (int i = 0; i < memberExceptions.Length; i++)
				{
					if (member == memberExceptions[i])
					{
						return true;
					}
				}
				for (int j = 0; j < nameExceptions.Length; j++)
				{
					if (member.Name == nameExceptions[j])
					{
						return true;
					}
				}
				if (includeObsolete)
				{
					return true;
				}
				object[] customAttributes = member.GetCustomAttributes(typeof(ObsoleteAttribute), inherit: true);
				return customAttributes.Length != 0;
			}
		}

		public class PropertyBuilderSettings : MemberBuilderSettings
		{
			public AccessorCreationBindings accessorCreationBindings { get; set; }

			public PropertyBuilderSettings()
			{
				accessorCreationBindings = AccessorCreationBindings.Both;
			}
		}

		[Flags]
		public enum AccessorCreationBindings
		{
			None = 0,
			Getter = 1,
			Setter = 2,
			Both = 3
		}

		[Flags]
		public enum StaticBindings
		{
			None = 0,
			Static = 1,
			Instance = 2,
			All = 3
		}

		[Flags]
		public enum AccesModifierBindings
		{
			None = 0,
			NonPublic = 1,
			Public = 2,
			All = 3
		}

		private Type type;

		private List<Command> commands = new List<Command>();

		public Command[] lastBuiltCommands => commands.ToArray();

		public PropertyBuilderSettings fieldsSettings { get; set; }

		public PropertyBuilderSettings propertiesSettings { get; set; }

		public MemberBuilderSettings methodsSettings { get; set; }

		public bool useClassName { get; set; }

		public string className { get; set; }

		public CommandsBuilder(Type type)
		{
			this.type = type;
			className = type.Name;
			fieldsSettings = new PropertyBuilderSettings();
			propertiesSettings = new PropertyBuilderSettings();
			methodsSettings = new MemberBuilderSettings();
		}

		public Command[] Build()
		{
			commands.Clear();
			BuildFields();
			BuildProperties();
			BuildMethods();
			return commands.ToArray();
		}

		private void BuildFields()
		{
			FieldInfo[] fields = GetFields(fieldsSettings);
			MethodInfo method = typeof(FieldInfo).GetMethod("SetValue", new Type[2]
			{
			typeof(object),
			typeof(object)
			});
			MethodInfo method2 = typeof(FieldInfo).GetMethod("GetValue", new Type[1] { typeof(object) });
			ConstantExpression constantExpression = Expression.Constant(null);
			for (int i = 0; i < fields.Length; i++)
			{
				if (fieldsSettings.accessorCreationBindings == AccessorCreationBindings.None)
				{
					continue;
				}
				ParameterExpression parameterExpression = Expression.Parameter(fields[i].FieldType, "value");
				ConstantExpression instance = Expression.Constant(fields[i]);
				if (fieldsSettings.accessorCreationBindings.HasFlag(AccessorCreationBindings.Getter))
				{
					try
					{
						MethodCallExpression expression = Expression.Call(instance, method2, constantExpression);
						UnaryExpression body = Expression.Convert(expression, fields[i].FieldType);
						Type delegateType = typeof(Func<>).MakeGenericType(fields[i].FieldType);
						LambdaExpression expression2 = Expression.Lambda(delegateType, body);
						ProcessFieldLamdaExpression(fields[i], expression2);
					}
					catch (Exception innerException)
					{
						CommandsManager.SendException(new CommandBuildingException(type, fields[i], innerException));
					}
				}
				if (fieldsSettings.accessorCreationBindings.HasFlag(AccessorCreationBindings.Setter) && !fields[i].IsInitOnly && !fields[i].IsLiteral)
				{
					try
					{
						UnaryExpression unaryExpression = Expression.Convert(parameterExpression, typeof(object));
						MethodCallExpression body2 = Expression.Call(instance, method, constantExpression, unaryExpression);
						Type delegateType2 = typeof(Action<>).MakeGenericType(fields[i].FieldType);
						LambdaExpression expression3 = Expression.Lambda(delegateType2, body2, parameterExpression);
						ProcessFieldLamdaExpression(fields[i], expression3);
					}
					catch (Exception innerException2)
					{
						CommandsManager.SendException(new CommandBuildingException(type, fields[i], innerException2));
					}
				}
			}
		}

		private void ProcessFieldLamdaExpression(FieldInfo field, LambdaExpression expression)
		{
			Delegate deleg = expression.Compile();
			Command item = new Command(deleg)
			{
				alias = field.Name,
				className = className,
				useClassName = useClassName
			};
			commands.Add(item);
		}

		private void BuildProperties()
		{
			PropertyInfo[] properties = GetProperties(propertiesSettings);
			for (int i = 0; i < properties.Length; i++)
			{
				if (propertiesSettings.accessorCreationBindings.HasFlag(AccessorCreationBindings.Getter) && properties[i].CanRead)
				{
					ProcessPropertyMethod(properties[i], properties[i].GetGetMethod(nonPublic: true));
				}
				if (propertiesSettings.accessorCreationBindings.HasFlag(AccessorCreationBindings.Setter) && properties[i].CanWrite)
				{
					ProcessPropertyMethod(properties[i], properties[i].GetSetMethod(nonPublic: true));
				}
			}
		}

		private void ProcessPropertyMethod(PropertyInfo property, MethodInfo method)
		{
			try
			{
				if ((method.IsPublic && propertiesSettings.accesModiferBindings.HasFlag(AccesModifierBindings.Public)) || (!method.IsPublic && propertiesSettings.accesModiferBindings.HasFlag(AccesModifierBindings.NonPublic)))
				{
					commands.Add(new MethodInfoCommand(method)
					{
						className = className,
						useClassName = useClassName
					});
				}
			}
			catch (Exception innerException)
			{
				CommandsManager.SendException(new CommandBuildingException(type, property, innerException));
			}
		}

		private void BuildMethods()
		{
			MethodInfo[] methods = GetMethods(methodsSettings);
			for (int i = 0; i < methods.Length; i++)
			{
				if (!methods[i].IsSpecialName)
				{
					try
					{
						commands.Add(new MethodInfoCommand(methods[i])
						{
							className = className,
							useClassName = useClassName
						});
					}
					catch (Exception innerException)
					{
						CommandsManager.SendException(new CommandBuildingException(type, methods[i], innerException));
					}
				}
			}
		}

		private FieldInfo[] GetFields(MemberBuilderSettings settings)
		{
			return GetMembersForSettings(settings, type.GetFields);
		}

		private PropertyInfo[] GetProperties(MemberBuilderSettings settings)
		{
			return GetMembersForSettings(settings, type.GetProperties);
		}

		private MethodInfo[] GetMethods(MemberBuilderSettings settings)
		{
			return GetMembersForSettings(settings, type.GetMethods);
		}

		private T[] GetMembersForSettings<T>(MemberBuilderSettings settings, Func<BindingFlags, T[]> callback) where T : MemberInfo
		{
			BindingFlags bindingFlagsForSettings = GetBindingFlagsForSettings(settings);
			List<T> list = callback(bindingFlagsForSettings).ToList();
			list.RemoveAll((T x) => settings.IsException(x));
			return list.ToArray();
		}

		private BindingFlags GetBindingFlagsForSettings(MemberBuilderSettings settings)
		{
			BindingFlags bindingFlags = BindingFlags.DeclaredOnly;
			if (settings.accesModiferBindings.HasFlag(AccesModifierBindings.Public))
			{
				bindingFlags |= BindingFlags.Public;
			}
			if (settings.accesModiferBindings.HasFlag(AccesModifierBindings.NonPublic))
			{
				bindingFlags |= BindingFlags.NonPublic;
			}
			if (settings.staticBindings.HasFlag(StaticBindings.Instance))
			{
				bindingFlags |= BindingFlags.Instance;
			}
			if (settings.staticBindings.HasFlag(StaticBindings.Static))
			{
				bindingFlags |= BindingFlags.Static;
			}
			return bindingFlags;
		}
	}
}