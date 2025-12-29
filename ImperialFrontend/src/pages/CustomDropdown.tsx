import FormControl from "@mui/material/FormControl";
import InputLabel from "@mui/material/InputLabel";
import MenuItem from "@mui/material/MenuItem";
import Select from "@mui/material/Select";
import { useCustomFormContext } from "../CustomFormContext";
import { useEffect, useState } from "react";
import axios from "axios";

interface DropdownProps {
  idColumn: string;
  displayColumn: string;
  label: string;
  apiEndpoint: string;
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
  idColumn,
  displayColumn,
  label,
  apiEndpoint,
}: DropdownProps) => {
  const { register, formValues } = useCustomFormContext();
  const [dropdownOptions, setDropdownOptions] = useState([]);

  useEffect(() => {
    axios.get(apiEndpoint).then((res) => {
      setDropdownOptions(res.data);
    });
  }, []);

  const el = (
    <FormControl fullWidth variant="outlined" sx={dropdownStyle}>
      <InputLabel id={`${idColumn}-label`}>{label}</InputLabel>
      <Select
        labelId={`${idColumn}-label`}
        id={idColumn}
        value={formValues[idColumn] ?? ""}
        label={label}
        onChange={(e) => register(idColumn, e.target.value)}
      >
        {dropdownOptions.map((option) => (
          <MenuItem key={option[idColumn]} value={option[idColumn]}>
            {option[displayColumn]}
          </MenuItem>
        ))}
      </Select>
    </FormControl>
  );
  return el;
};
