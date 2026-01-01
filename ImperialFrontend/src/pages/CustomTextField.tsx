import TextField from "@mui/material/TextField";
import { useCustomFormContext } from "../CustomFormContext";

//TODO: Add mui form control props

interface TextFieldProps {
  id: string;
  label: string;
  required?: boolean;
}

const textFieldStyle = {
  "& .MuiOutlinedInput-root": {
    // default outline
    "& fieldset": {
      borderColor: "#000000",
    },

    // hover
    "&:hover fieldset": {
      borderColor: "#000000ff",
    },

    // focused
    "&.Mui-focused fieldset": {
      borderColor: "#000000ff",
    },
  },

  // label color (default)
  "& .MuiInputLabel-root": {
    color: "#000000",
  },

  // label when focused
  "& .MuiInputLabel-root.Mui-focused": {
    color: "#000000ff",
  },
  "& .MuiOutlinedInput-root.Mui-error fieldset": {
    borderColor: "#d32f2f",
  },
  "& .MuiInputLabel-root.Mui-error": {
    color: "#d32f2f",
  },
};

export const CustomTextField = ({ id, label, required }: TextFieldProps) => {
  const { register, formValues } = useCustomFormContext();
  const el = (
    <>
      <TextField
        id={id}
        label={label}
        variant="outlined"
        required={required}
        fullWidth
        sx={textFieldStyle}
        onChange={e => register(id, e.target.value)}
        value={formValues[id] ?? ""}
      />
    </>
  );
  return el;
};
