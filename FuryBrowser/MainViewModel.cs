using System;
using System.Reactive;
using ReactiveUI;

namespace FuryBrowser;

public class MainViewModel : ReactiveObject
{
	private Uri? _destination;
	public Uri? Destination
	{
		get => _destination;
		set => this.RaiseAndSetIfChanged(ref _destination, value);
	}

	private string? _omnibarText;
	public string? OmnibarText
	{
		get => _omnibarText;
		set => this.RaiseAndSetIfChanged(ref _omnibarText, value);
	}

	public MainViewModel()
	{
		Unit Action(string? destination)
		{
			var uri = MainView.InterpretOmnibarText(destination ?? OmnibarText);
			Destination = uri ?? new Uri(destination ?? OmnibarText);

			return Unit.Default;
		}

		GoToPage = ReactiveCommand.Create((Func<string, Unit>)Action /* TODO: when can we execute, as IObservable */);
	}

	public ReactiveCommand<string, Unit> GoToPage { get; }
}
