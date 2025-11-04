# Golden Files

Golden files are reference JSON samples used for contract testing and schema validation.

## Files

- `patient_state_request_v0.1.json` - Sample PatientStateRequest in v0.1 format
- `patient_state_response_v0.1.json` - Sample PatientStateResponse in v0.1 format

## Usage

These files are used by `PatientStateContractTests` to:
1. Verify JSON serialization/deserialization round-trips correctly
2. Detect breaking changes in schema
3. Validate that contracts maintain backward compatibility

## Schema Versioning

When making breaking changes to the schema:
1. Create new golden files with the new version (e.g., `patient_state_request_v0.2.json`)
2. Update contract tests to test both versions
3. Document migration path in the contract tests

## Requirements

- All units must be in SI (meters, kilograms, seconds, etc.)
- Timestamps must be ISO 8601 format (UTC)
- No PII (personally identifiable information) should be included
- All numeric values should be realistic medical ranges
