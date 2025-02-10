# Postcode-To-Country

Adds a new column to csv/tsv files with a country found from a postcode column.

Uses [postcodes.io](https://postcodes.io/) for postcode lookups.

## Usage

```bash
> ./GetCountries --help

DESCRIPTION:
Adds a new column to csv/tsv files with a country found from a postcode column.

USAGE:
    GetCountries <INPUTFILE> [OPTIONS]

ARGUMENTS:
    <INPUTFILE>    Required. Absolute or relative path to csv or tsv file. 

OPTIONS:
                                    DEFAULT                                                                   
    -h, --help                                  Prints help information                                       
    -o, --output <FILENAME>                     Optional. Output filename with extension or without extension.
                                                Can be used to convert between valid file formats.            
    -c, --column <COLUMN>           postcode    Optional. Postcode column column name. Case-sensitive.        
    -n, --new-column <NEWCOLUMN>    country     Optional. Name of appended country column.                    

  
``` 

```bash
# success!
> ./GetCountries ./path/to/file.csv
Starting postcode lookup...
Output file written to:
./path/to/file-with-countries.csv

> ./GetCountries ./path/to/file.csv -o new-file.tsv
Starting postcode lookup...
Output file written to:
./path/to/new-file.tsv
```

```bash
# failures
> ./GetCountries ./path/to/file.txt
Error: Input file must be .csv or .tsv. Found .txt.

> ./GetCountries /path/to/missing/file.csv
Error: Please specify a valid input file. "./path/to/file.csv"
```

## Example

```csv
# /home/me/files/inputfile.csv
foo, bar, postcode, baz
  1,   2, SW19 5AE,   4
```

```bash
> ./GetCountries /home/me/files/inputfile.csv
Starting postcode lookup...
Output file written to:
/home/me/inputfile-with-countries.csv
```

```csv
# /home/me/files/inputfile-with-countries.csv
foo, bar, postcode, baz, country
  1,   2, SW19 5AE,   4, England
```
