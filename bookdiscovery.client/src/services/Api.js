const API_BASE = import.meta.env.VITE_API_BASE_URL;
export async function searchBooks(query) {
    const res = await fetch(API_BASE + "/api/books/search", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ query })
    });
    if (!res.ok) {
        console.error("Failed to search books");
        return [];
    }
    return await res.json();
}
