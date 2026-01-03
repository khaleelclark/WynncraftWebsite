import FormControl from "@mui/material/FormControl";
import InputLabel from "@mui/material/InputLabel";
import MenuItem from "@mui/material/MenuItem";
import Select from "@mui/material/Select";
import { useCustomFormContext } from "../CustomFormContext";
import { useEffect, useState } from "react";
import { adminApi } from "../api";
import Autocomplete from "@mui/material/Autocomplete";
import TextField from "@mui/material/TextField";

interface DropdownProps {
  id: string;
  label: string;
  apiEndpoint: string;
  multiple?: boolean;
}

export const CustomDropdown = ({
  id,
  label,
  apiEndpoint,
  multiple,
}: DropdownProps) => {
  const { register, formValues } = useCustomFormContext();
  const [dropdownOptions, setDropdownOptions] = useState([]);

  useEffect(() => {
    adminApi.get(apiEndpoint).then(res => {
      setDropdownOptions(res.data);
    });
  }, []);
  const el = multiple ? (
    <Autocomplete
      value={formValues[id] ?? []}
      onChange={(e, selectedItem) => {
        register(id, selectedItem);
      }}
      getOptionLabel={option => option?.name ?? "Not Selected"}
      id={id}
      options={dropdownOptions}
      fullWidth
      filterSelectedOptions
      isOptionEqualToValue={(o, v) => o.id === v.id}
      renderInput={params => <TextField {...params} label={label} />}
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
      value={formValues[id] ?? ""}
      onChange={(e, selectedItem) => {
        register(id, selectedItem);
      }}
      getOptionLabel={option => option?.name ?? "Not Selected"}
      id={id}
      options={dropdownOptions}
      fullWidth
      renderInput={params => <TextField {...params} label={label} />}
    />
  );
  return el;
};
