using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;

namespace DeadlyReentry
{
	[KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
	public class DRToolbar : UnityEngine.MonoBehaviour
	{
		private static Rect windowPosition = new Rect(0,0,360,480);
		private static GUIStyle windowStyle = null;
		
		#region Fields
		private GUISkin skins = HighLogic.Skin;
		private int id = Guid.NewGuid().GetHashCode();
		//private bool visible = false, showing = true;
		private Rect window = new Rect(), button = new Rect();
		private Texture2D buttonTexture = new Texture2D(1, 1);
		#endregion
		
		#region Properties
		private GUIStyle _buttonStyle = null;
		private GUIStyle buttonStyle
		{
			get
			{
				if (_buttonStyle == null)
				{
					_buttonStyle = new GUIStyle(skins.button);
					_buttonStyle.onNormal = _buttonStyle.hover;
				}
				return _buttonStyle;
			}
		}
		#endregion
		
		private ApplicationLauncherButton DRToolbarButton = null;
		private bool visible = false;
		
		public static DRToolbar Instance
		{
			get;
			private set;
		}
		
		public DRToolbar ()
		{
			if (Instance == null) 
			{
				Instance = this;
			}
		}
		public void Awake() 
		{
			// Set up the stock toolbar
			//GameEvents.onGUIApplicationLauncherReady.Add(OnGUIAppLauncherReady);
			//GameEvents.onGUIApplicationLauncherDestroyed.Add(OnGUIAppLauncherDestroyed);			
		}
		
		public void Start() 
		{
			// Debug.Log("CLSAddon:Start");
			
			windowStyle = new GUIStyle (HighLogic.Skin.window);
			
			try 
			{
				RenderingManager.RemoveFromPostDrawQueue (0, OnDraw);
			} 
			catch 
			{
				// This is generally not a problem - do not log it.
				// Debug.LogException(ex);
			}
			
			//if (HighLogic.LoadedScene.Equals( SpaceCenter) )
			//{
			RenderingManager.AddToPostDrawQueue (0, OnDraw);
			//}
		}
		
		void OnGUIAppLauncherReady()
		{
			if (ApplicationLauncher.Ready)
			{
				this.DRToolbarButton = ApplicationLauncher.Instance.AddModApplication(onAppLaunchToggleOn,
				                                                                       onAppLaunchToggleOff,
				                                                                       DummyVoid,
				                                                                       DummyVoid,
				                                                                       DummyVoid,
				                                                                       DummyVoid,
				                                                                       ApplicationLauncher.AppScenes.SPACECENTER,
				                                                                       (Texture)GameDatabase.Instance.GetTexture("DeadlyReentry/Assets/DR_icon_off", false));
			}
		}
		
		void OnGUIAppLauncherDestroyed()
		{
			if (this.DRToolbarButton != null)
			{
				ApplicationLauncher.Instance.RemoveModApplication(this.DRToolbarButton);
				this.DRToolbarButton = null;
			}
		}
		
		void onAppLaunchToggleOn()
		{
			this.DRToolbarButton.SetTexture((Texture)GameDatabase.Instance.GetTexture("DeadlyReentry/Assets/DR_icon_on", false));
			this.visible = true;
		}
		
		void onAppLaunchToggleOff()
		{
			this.DRToolbarButton.SetTexture((Texture)GameDatabase.Instance.GetTexture("DeadlyReentry/Assets/DR_icon_off", false));
			
			this.visible = false;
		}
		
		
		void DummyVoid()
		{
		}
		private void OnDraw()
		{
			if (this.visible)
			{
				//Set the GUI Skin
				//GUI.skin = HighLogic.Skin;
				
				windowPosition = GUILayout.Window(id, windowPosition, OnWindow, "Deadly Reentry Settings", windowStyle,GUILayout.MinHeight(20),GUILayout.ExpandHeight(true));
			}
		}
		public void OnDestroy()
		{
			
			// Remove the stock toolbar button
			GameEvents.onGUIApplicationLauncherReady.Remove(OnGUIAppLauncherReady);
			if (this.DRToolbarButton != null)
				ApplicationLauncher.Instance.RemoveModApplication(DRToolbarButton);			
		}
		
		private void OnWindow(int windowID)
		{
			string[] difficulties = {"Easy", "Normal", "Hard"};
			DeadlyReentryScenario.Instance.DifficultySetting = GUILayout.SelectionGrid (DeadlyReentryScenario.Instance.DifficultySetting, difficulties, 4);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			GUI.DragWindow();
		}
	}
}