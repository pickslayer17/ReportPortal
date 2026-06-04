// The ONLY place the frontend learns the backend URL.
// Value comes exclusively from the VITE_API_URL env var:
//   - dev:    .env.development
//   - deploy: real environment variable set before `vite build`
// No URL is hardcoded anywhere in the app — fail fast if it is missing.
const apiUrl = import.meta.env.VITE_API_URL as string | undefined;

if (!apiUrl) {
    throw new Error(
        'VITE_API_URL is not set. Define it in .env.development (dev) or in the deploy environment.'
    );
}

export const API_URL: string = apiUrl;
