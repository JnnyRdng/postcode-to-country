# Postcode-To-Country

Given a csv file with a column named `postcode` (containing a valid UK postcode!) outputs a new csv file with a `country` column appended.

Uses [postcodes.io](https://postcodes.io/) for postcode lookups.

## Usage

A single argument is **required**. Must be a relative or absolute path to a csv file.

```bash
# success!
> ./GetCountries ./path/to/file.csv
Updated CSV written to /absolute/path/to/countries-file.csv
```

```bash
# failures
> ./GetCountries
Enter a filepath to a csv!
> ./GetCountries ./path/to/file.txt
Invalid file format! .txt
> ./GetCountries /path/to/missing/file.csv
File does not exist! '/path/to/missing/file.csv'
```

## Example

```csv
# /home/me/files/inputfile.csv
foo, bar, postcode, baz
  1,   2, SW19 5AE,   4
```

```bash
> ./GetCountries /home/me/files/inputfile.csv
Updated CSV written to /home/me/countries-inputfile.csv
```

```csv
# /home/me/files/countries-inputfile.csv
foo, bar, postcode, baz, country
  1,   2, SW19 5AE,   4, England
```
