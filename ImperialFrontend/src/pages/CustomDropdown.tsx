import { useEffect, useState } from "react";
import { adminApi } from "../api";
import Autocomplete from "@mui/material/Autocomplete";
import TextField from "@mui/material/TextField";
import { FormRegisterProps } from "./CustomForm";

type DropdownProps = {
  apiEndpoint: string;
  multiple?: boolean;
  required?: boolean;
} & FormRegisterProps;

export const CustomDropdown = ({
  id,
  label,
  apiEndpoint,
  multiple,
  register,
  required = true,
  formValues,
  formValidations,
}: DropdownProps) => {
  const [dropdownOptions, setDropdownOptions] = useState([]);
  const [touched, setTouched] = useState(false);
  const showError = required && touched && !formValidations?.[id];

  useEffect(() => {
    adminApi.get(apiEndpoint).then(res => {
      setDropdownOptions(res.data);
    });
    const value = formValues?.[id] ?? (multiple ? [] : null);

    const hasValue = multiple
      ? Array.isArray(value) && value.length > 0
      : !!value;

    const isValid = required ? hasValue : true;

    register?.(id, value, isValid);
  }, []);

  const el = multiple ? (
    <Autocomplete
      value={formValues?.[id] ?? []}
      onChange={(_, selectedItem: any) => {
        const hasValue = multiple
          ? Array.isArray(selectedItem) && selectedItem.length > 0
          : !!selectedItem;

        const validated = required ? hasValue : true;
        register?.(id, selectedItem, validated);
        setTouched(true);
      }}
      getOptionLabel={option => option?.name ?? "Not Selected"}
      id={id}
      options={dropdownOptions}
      fullWidth
      filterSelectedOptions
      isOptionEqualToValue={(o, v) => o.id === v.id}
      renderInput={params => (
        <TextField
          {...params}
          label={label}
          error={showError}
          helperText={showError ? "This field is required" : ""}
        />
      )}
      multiple={multiple}
      sx={{
        "& .MuiChip-root": {
          bgcolor: theme => theme.palette.primary.main + "33",
          color: theme => theme.palette.text.primary,
          border: theme => `1px solid ${theme.palette.primary.main}`,
          borderRadius: 6,
        },
        "& .MuiChip-deleteIcon": {
          color: theme => theme.palette.text.secondary,
          "&:hover": {
            color: theme => theme.palette.error.main,
          },
        },
        "& .MuiInputBase-input": {
          color: theme => theme.palette.text.primary,
        },
      }}
    />
  ) : (
    <Autocomplete
      value={formValues?.[id] ?? null}
      onChange={(_, selectedItem) => {
        const hasValue = !!selectedItem;
        const validated = required ? hasValue : true;
        register?.(id, selectedItem, validated);
        setTouched(true);
      }}
      getOptionLabel={option => option?.name ?? "Not Selected"}
      id={id}
      options={dropdownOptions}
      fullWidth
      renderInput={params => (
        <TextField
          {...params}
          label={label}
          error={showError}
          helperText={showError ? "This field is required" : ""}
        />
      )}
    />
  );
  return el;
};
