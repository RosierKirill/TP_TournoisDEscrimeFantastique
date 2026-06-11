import type { Config } from "tailwindcss";

const config: Config = {
  darkMode: ["class"],
  content: [
    "./src/**/*.{ts,tsx}",
  ],
  theme: {
    container: {
      center: true,
      padding: "1.5rem",
      screens: { "2xl": "1200px" },
    },
    extend: {
      colors: {
        border: "hsl(var(--border))",
        input: "hsl(var(--input))",
        ring: "hsl(var(--ring))",
        background: "hsl(var(--background))",
        foreground: "hsl(var(--foreground))",
        primary: {
          DEFAULT: "hsl(var(--primary))",
          foreground: "hsl(var(--primary-foreground))",
        },
        secondary: {
          DEFAULT: "hsl(var(--secondary))",
          foreground: "hsl(var(--secondary-foreground))",
        },
        muted: {
          DEFAULT: "hsl(var(--muted))",
          foreground: "hsl(var(--muted-foreground))",
        },
        accent: {
          DEFAULT: "hsl(var(--accent))",
          foreground: "hsl(var(--accent-foreground))",
        },
        destructive: {
          DEFAULT: "hsl(var(--destructive))",
          foreground: "hsl(var(--destructive-foreground))",
        },
        card: {
          DEFAULT: "hsl(var(--card))",
          foreground: "hsl(var(--card-foreground))",
        },
        gold: {
          DEFAULT: "hsl(var(--gold))",
          foreground: "hsl(var(--gold-foreground))",
        },
      },
      borderRadius: {
        lg: "var(--radius)",
        md: "calc(var(--radius) - 2px)",
        sm: "calc(var(--radius) - 4px)",
      },
      fontFamily: {
        display: ["var(--font-cinzel)", "serif"],
        sans: ["var(--font-inter)", "system-ui", "sans-serif"],
      },
      keyframes: {
        "fade-up": {
          from: { opacity: "0", transform: "translateY(8px)" },
          to: { opacity: "1", transform: "translateY(0)" },
        },
        shimmer: {
          "0%": { backgroundPosition: "-200% 0" },
          "100%": { backgroundPosition: "200% 0" },
        },
        "lunge-left": {
          "0%, 100%": { transform: "translateX(0) rotate(0deg)" },
          "40%": { transform: "translateX(38%) rotate(8deg)" },
          "55%": { transform: "translateX(30%) rotate(6deg)" },
        },
        "lunge-right": {
          "0%, 100%": { transform: "translateX(0) rotate(0deg) scaleX(-1)" },
          "40%": { transform: "translateX(-38%) rotate(-8deg) scaleX(-1)" },
          "55%": { transform: "translateX(-30%) rotate(-6deg) scaleX(-1)" },
        },
        clash: {
          "0%, 35%, 60%, 100%": { opacity: "0", transform: "translate(-50%, -50%) scale(0.6)" },
          "45%, 52%": { opacity: "1", transform: "translate(-50%, -50%) scale(1.15)" },
        },
        "shake-x": {
          "0%, 100%": { transform: "translateX(0)" },
          "20%, 60%": { transform: "translateX(-5px)" },
          "40%, 80%": { transform: "translateX(5px)" },
        },
        "pop-in": {
          "0%": { opacity: "0", transform: "scale(0.8)" },
          "100%": { opacity: "1", transform: "scale(1)" },
        },
      },
      animation: {
        "fade-up": "fade-up 0.4s ease-out both",
        shimmer: "shimmer 3s linear infinite",
        "lunge-left": "lunge-left 0.9s ease-in-out infinite",
        "lunge-right": "lunge-right 0.9s ease-in-out infinite",
        clash: "clash 0.9s ease-in-out infinite",
        "shake-x": "shake-x 0.5s ease-in-out",
        "pop-in": "pop-in 0.35s ease-out both",
      },
    },
  },
  plugins: [require("tailwindcss-animate")],
};

export default config;
