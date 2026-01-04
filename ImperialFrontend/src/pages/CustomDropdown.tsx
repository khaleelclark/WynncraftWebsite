import { useEffect, useState } from "react";
import { adminApi } from "../api";
import Autocomplete from "@mui/material/Autocomplete";
import TextField from "@mui/material/TextField";
import { FormRegisterProps } from "./CustomForm";

type DropdownProps = {
  apiEndpoint: string;
  multiple?: boolean;
} & FormRegisterProps;

export const CustomDropdown = ({
  id,
  label,
  apiEndpoint,
  multiple,
  register,
  formValues,
  formValidations,
}: DropdownProps) => {
  const [dropdownOptions, setDropdownOptions] = useState([]);
  const [touched, setTouched] = useState(false);

  useEffect(() => {
    adminApi.get(apiEndpoint).then(res => {
      setDropdownOptions(res.data);
    });
    register?.(
      id,
      formValues?.[id] ? formValues?.[id] : multiple ? [] : "",
      formValues?.id !== undefined
    );
  }, []);

  const el = multiple ? (
    <Autocomplete
      value={formValues?.[id] ?? []}
      onChange={(_, selectedItem: any) => {
        const validated =
          Array.isArray(selectedItem) && selectedItem.length > 0;

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
          error={!formValidations?.[id] && touched}
          helperText={
            !formValidations?.[id] && touched ? "This field is required" : ""
          }
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
        const validated = !!selectedItem;
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
          error={!formValidations?.[id] && touched}
          helperText={
            !formValidations?.[id] && touched ? "This field is required" : ""
          }
        />
      )}
    />
  );
  return el;
};
