# FluentValidationToJsonSchema

## Overview
`FluentValidationToJsonSchema` is a library designed to convert FluentValidation rules into JSON Schema definitions. This is particularly useful for generating schema-based validation for APIs or other systems that rely on JSON Schema.

## Features
- Converts FluentValidation rules to JSON Schema.
- Supports `.NET 6`, `.NET 7`, `.NET 8`, and `.NET Standard 2.0`.
- Handles complex validation rules and custom validators.
- Lightweight and easy to integrate into existing projects.

## Installation
To install the library, use the NuGet Package Manager:
## Usage
Here is a basic example of how to use the library:
## Supported Validation Rules
The library supports the following FluentValidation rules:
- `NotEmpty`
- `NotNull`
- `InclusiveBetween`
- `ExclusiveBetween`
- `Length`
- `Matches` (Regex)
- Custom validators (with limited support)

## Contributing
Contributions are welcome! Please fork the repository and submit a pull request with your changes. Ensure all new code is covered by unit tests.

## License
This project is licensed under the MIT License. See the `LICENSE` file for details.

## Contact
For questions or support, please open an issue in the GitHub repository.
