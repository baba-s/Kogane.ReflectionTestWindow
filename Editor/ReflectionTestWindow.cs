using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Kogane.Internal
{
    internal sealed class ReflectionTestWindow : EditorWindow
    {
        [SerializeField] private string       m_assemblyName;
        [SerializeField] private string       m_namespaceName;
        [SerializeField] private string       m_typeName;
        [SerializeField] private BindingFlags m_bindingFlags;
        [SerializeField] private string       m_result;
        [SerializeField] private Vector2      m_scrollPosition;

        [MenuItem( "Window/Kogane/Reflection Test", false, 1522421635 )]
        private static void Open()
        {
            GetWindow<ReflectionTestWindow>( "Reflection Test" );
        }

        private void OnGUI()
        {
            m_assemblyName  = EditorGUILayout.TextField( "Assembly Name", m_assemblyName );
            m_namespaceName = EditorGUILayout.TextField( "Namespace Name", m_namespaceName );
            m_typeName      = EditorGUILayout.TextField( "Type Name", m_typeName );
            m_bindingFlags  = ( BindingFlags )EditorGUILayout.EnumFlagsField( "Binding Flags", m_bindingFlags );

            using ( new EditorGUILayout.HorizontalScope() )
            {
                if ( GUILayout.Button( "GetFields" ) )
                {
                    UpdateResult( x => x.GetFields( m_bindingFlags ) );
                }

                if ( GUILayout.Button( "GetProperties" ) )
                {
                    UpdateResult( x => x.GetProperties( m_bindingFlags ) );
                }

                if ( GUILayout.Button( "GetMethods" ) )
                {
                    UpdateResult( x => x.GetMethods( m_bindingFlags ) );
                }
            }

            using ( var scope = new EditorGUILayout.ScrollViewScope( m_scrollPosition ) )
            {
                var style = new GUIStyle( EditorStyles.label )
                {
                    alignment = TextAnchor.UpperLeft,
                };
                var size = style.CalcSize( new GUIContent( m_result ) );
                EditorGUILayout.SelectableLabel( m_result, style, GUILayout.Height( size.y ) );
                m_scrollPosition = scope.scrollPosition;
            }
        }

        private void UpdateResult( Func<Type, IReadOnlyList<MemberInfo>> func )
        {
            var type = Type.GetType( $"{m_namespaceName}.{m_typeName},{m_assemblyName}" );

            if ( type == null )
            {
                m_result = "Type not found";
                return;
            }

            var members = func( type );

            m_result = string.Join( "\n", members.Select( x => x.Name ).Distinct().OrderBy( x => x ) );
        }
    }
}