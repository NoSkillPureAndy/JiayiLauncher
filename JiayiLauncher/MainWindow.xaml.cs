﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Blazored.Modal;
using JiayiLauncher.Features.Mods;
using JiayiLauncher.Settings;
using JiayiLauncher.Utils;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Extensions.DependencyInjection;

namespace JiayiLauncher;

public partial class MainWindow
{
	[DllImport("dwmapi.dll", PreserveSig = true)]
	private static extern int DwmSetWindowAttribute(nint hWnd, int attr, ref bool attrValue, int attrSize);
	
	public MainWindow()
	{
		InitializeComponent();
		Log.CreateLog();
		
		SourceInitialized += DarkTitlebar;
		
		AppDomain.CurrentDomain.UnhandledException += (_, args) =>
		{
			Log.Write(args.ExceptionObject, ((Exception)args.ExceptionObject).ToString(), Log.LogLevel.Error);
			MessageBox.Show(
				"Jiayi has ran into a problem and needs to close. Please send your log file to the nearest developer.",
				"Crash", MessageBoxButton.OK, MessageBoxImage.Error
			);
		};

		var services = new ServiceCollection();
		services.AddWpfBlazorWebView();
		services.AddBlazoredModal();
#if DEBUG
		services.AddBlazorWebViewDeveloperTools();
#endif
		Resources.Add("services", services.BuildServiceProvider());
		
		// startup stuff
		JiayiSettings.Load();
		if (JiayiSettings.Instance!.ModCollectionPath != string.Empty)
		{
			ModCollection.Load(JiayiSettings.Instance.ModCollectionPath);
		}
	}

	private void DarkTitlebar(object? sender, EventArgs e)
	{
		var windowHelper = new WindowInteropHelper(this);
		var value = true;
		var result = DwmSetWindowAttribute(windowHelper.Handle, 20, ref value, Marshal.SizeOf(value));
		if (result != 0) Log.Write(this, $"Failed to set dark titlebar. Error code: {result}", Log.LogLevel.Warning);
	}

	// ReSharper disable once UnusedMember.Local
	private void ChangeColor(object? sender, BlazorWebViewInitializedEventArgs e)
	{
		e.WebView.DefaultBackgroundColor = Color.FromArgb(15, 15, 15);
	}
}