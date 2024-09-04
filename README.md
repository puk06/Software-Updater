# Software Updater

A simple and lightweight tool to automate the process of downloading the latest version of a GitHub repository. 

## Features

- **Automatic Downloads:** Fetches the specified version of a GitHub repository.
- **Easy to Use:** Simple command-line interface.
- **Flexible:** Works with any GitHub repository.

## Usage

To use the Software Updater, run `Software Updater.exe` with the following arguments:

```php
Software Updater.exe <GitHub Tag> <GitHub Username> <Repository Name>
```

### Arguments

- `<GitHub Tag>`: The specific tag or release version you want to download.
- `<GitHub Username>`: The GitHub username of the repository owner.
- `<Repository Name>`: The name of the GitHub repository.

### Example

If you want to download the `v1.0.0` release from the repository `example-repo` owned by the user `example-user`, you would run:

```php
"Software Updater.exe" v1.0.0 example-user example-repo
```

### Download Location

The downloaded data will be stored in the parent directory of the folder where `Software Updater.exe` is located.

## Requirements

- Windows operating system.
- Internet connection.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

