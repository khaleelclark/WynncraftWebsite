import FormControl from "@mui/material/FormControl";
import InputLabel from "@mui/material/InputLabel";
import MenuItem from "@mui/material/MenuItem";
import Select from "@mui/material/Select";
import { useCustomFormContext } from "../CustomFormContext";
import { useEffect, useState } from "react";
import axios from "axios";

interface DropdownProps {
  id: string;
  label: string;
  apiEndpoint: string;
  multiple?: boolean;
}

const dropdownStyle = {
  // label text + asterisk
  "& .MuiInputLabel-root": {
    color: "#000000",
  },

  // selected value text (closed select)
  "& .MuiSelect-select": {
    color: "#ffffff",
  },

  // dropdown arrow
  "& .MuiSelect-icon": {
    color: "#000000",
  },

  // outline border
  "& .MuiOutlinedInput-notchedOutline": {
    borderColor: "#000000",
  },
  "&:hover .MuiOutlinedInput-notchedOutline": {
    borderColor: "#000000",
  },
  "& .MuiOutlinedInput-root.Mui-focused .MuiOutlinedInput-notchedOutline": {
    borderColor: "#000000",
  },
};
export const CustomDropdown = ({
  id,
  label,
  apiEndpoint,
  multiple = false,
}: DropdownProps) => {
  const { register, formValues } = useCustomFormContext();
  const [dropdownOptions, setDropdownOptions] = useState(
    formValues[id] ? [formValues[id]] : []
  );

  useEffect(() => {
    axios.get(apiEndpoint).then(res => {
      setDropdownOptions(res.data);
    });
  }, []);
  const el = multiple ? (
    <FormControl fullWidth variant="outlined" sx={dropdownStyle}>
      <InputLabel id={`${id}-label`}>{label}</InputLabel>
      <Select
        multiple
        labelId={`${id}-label`}
        id={id}
        label={label}
        value={formValues[id] ?? []}
        onChange={e => {
          const selectedIds = e.target.value;

          const selectedObjects = dropdownOptions.filter(option =>
            selectedIds.includes(option.id)
          );

          register(id, selectedObjects);
        }}
      >
        {dropdownOptions.map(option => (
          <MenuItem key={option.id} value={option.id}>
            {option.name}
          </MenuItem>
        ))}
      </Select>
    </FormControl>
  ) : (
    <FormControl fullWidth variant="outlined" sx={dropdownStyle}>
      <InputLabel id={`${id}-label`}>{label}</InputLabel>
      <Select
        labelId={`${id}-label`}
        id={id}
        value={formValues[id] ? formValues[id].id ?? "" : ""}
        label={label}
        onChange={e => {
          const selected = dropdownOptions.find(
            option => option.id === e.target.value
          );
          register(id, selected);
        }}
      >
        {dropdownOptions.map((option: any) => (
          <MenuItem key={option.id} value={option.id}>
            {option.name}
          </MenuItem>
        ))}
      </Select>
    </FormControl>
  );
  return el;
};
