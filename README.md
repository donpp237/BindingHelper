# BindingHelper

## Purpose

Helper that can assist in binding processing in MVVM patterns

## Example

### View
```xml
<Window x:Class="MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:FrameworkTest"
        xmlns:viewModels="clr-namespace:ViewModels"
        mc:Ignorable="d"
        Title="MainWindow" Height="450" Width="800">

    <Window.Resources>
        <viewModels:ViewModel x:Key="viewModel"/>
    </Window.Resources>
    
    <Grid DataContext="{Binding Source={StaticResource viewModel}}">
        <Grid.RowDefinitions>
            <RowDefinition/>
            <RowDefinition/>
        </Grid.RowDefinitions>
        
        <TextBox Grid.Row="0" Width="200" Height="200"
                 VerticalAlignment="Center"
                 HorizontalAlignment="Center"
                 Text="{Binding Name}"/>

        <Button Grid.Row="1"
                Content="click"
                Command="{Binding Click}"/>
    </Grid>
</Window>
```

### ViewModel


```c#
class ViewModel : BindingHelper.Helper
{
    // Property
    public string Name
    { 
        get => GetProperty<string>();
        set => SetProperty<string>(value);
    }
    
    // Command
    public ICommand Click
    { 
        get => GetCommand((x) => Update()); 
    }

    // Command Trigger Function
    private void Update()
    {
        Name = "Hello World";
    }
}
```
