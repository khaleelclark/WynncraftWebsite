import FormControl from "@mui/material/FormControl";
import InputLabel from "@mui/material/InputLabel";
import MenuItem from "@mui/material/MenuItem";
import Select from "@mui/material/Select";
import { useCustomFormContext } from "../CustomFormContext";
import { useEffect, useState } from "react";
import axios from "axios";
import Autocomplete from "@mui/material/Autocomplete";
import TextField from "@mui/material/TextField";

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
  multiple,
}: DropdownProps) => {
  const { register, formValues } = useCustomFormContext();
  const [dropdownOptions, setDropdownOptions] = useState([]);

  useEffect(() => {
    axios.get(apiEndpoint).then(res => {
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
      renderInput={params => (
        <TextField sx={dropdownStyle} {...params} label={label} />
      )}
      multiple={multiple}
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
      renderInput={params => (
        <TextField sx={dropdownStyle} {...params} label={label} />
      )}
    />
  );
  return el;
};
