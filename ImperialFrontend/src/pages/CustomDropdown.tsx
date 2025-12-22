import FormControl from "@mui/material/FormControl";
import InputLabel from "@mui/material/InputLabel";
import MenuItem from "@mui/material/MenuItem";
import Select, { SelectChangeEvent } from "@mui/material/Select";

interface DropdownProps {
  id: string;
  label: string;
  dropdownOptions: string[];
  required?: boolean;
  value: string;
  onChange: (value: string) => void;
}

const dropdownStyle = {
  // label text + asterisk
  "& .MuiInputLabel-root": {
    color: "#000000",
  },
  "& .MuiInputLabel-root.Mui-focused": {
    color: "#000000",
  },
  "& .MuiFormLabel-asterisk": {
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
  dropdownOptions,
  value,
  onChange,
  required,
}: DropdownProps) => {
  let el = (
    <FormControl
      fullWidth
      required={required}
      variant="outlined"
      sx={dropdownStyle}
    >
      <InputLabel id={`${id}-label`}>{label}</InputLabel>

      <Select
        labelId={`${id}-label`}
        id={id}
        value={value}
        label={label}
        onChange={(e: SelectChangeEvent) => onChange(e.target.value)}
      >
        {dropdownOptions.map((option) => (
          <MenuItem key={option} value={option}>
            {option}
          </MenuItem>
        ))}
      </Select>
    </FormControl>
  );
  return el;
};
