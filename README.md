# Software Updater

A simple and lightweight tool to automate the process of downloading the latest version of a GitHub repository. 

## Features

- **Automatic Downloads:** Fetches the specified version of a GitHub repository.
- **Easy to Use:** Simple command-line interface.
- **Flexible:** Works with any GitHub repository.

## Usage

To use the Software Updater, run `Software Updater.exe` with the following arguments:

```php
Software Updater.exe <GitHub Tag> <GitHub Username> <Repository Name> <Executable FileName> ...<Ignore Files>
```

### Arguments

- `<GitHub Tag>`: The specific tag or release version you want to download.
- `<GitHub Username>`: The GitHub username of the repository owner.
- `<Repository Name>`: The name of the GitHub repository.
- `<Executable FileName>`: The name of the Executable Filename.
- `...<Ignore Files>`: The name of the Ignore Filename.

### Example

If you want to download the `v1.0.0` release from the repository `example-repo` owned by the user `example-user`, and the executable file is named `example-app.exe`, you would run:

```php
"Software Updater.exe" "v1.0.0" "example-user" "example-repo" "example-app" "ignore.1" "ignore.2"
```

### Download Location

The downloaded data will be stored in the parent directory of the folder where `Software Updater.exe` is located.

## Important Note

For the updater to work properly, ensure that the executable file you wish to update is located in the parent directory of the folder containing `Software Updater.exe`. For example, if the executable is in the `/Updater` folder, the folder structure should be as follows:

```markdown
/YourAppFolder
    /Updater
        Software Updater.exe
    your-app.exe
```

In this setup, `Software Updater.exe` is in the `/Updater` folder, and the executable you want to update (`your-app.exe`) is in the parent folder.

## Requirements

- Windows operating system.
- Internet connection.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

