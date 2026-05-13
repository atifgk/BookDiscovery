import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { useEffect, useState } from 'react';
import './App.css';
import { searchBooks } from "./services/Api";
function App() {
    const [books, setBooks] = useState();
    const [query, setQuery] = useState('');
    const [loading, setLoading] = useState(false);
    const [isDataSearched, setIsDataSearched] = useState(false);
    useEffect(() => {
    }, []);
    const contents = loading ? (_jsxs("div", { className: "text-center my-4", children: [_jsx("div", { className: "spinner-border text-primary", role: "status" }), _jsx("p", { className: "mt-2", children: "Loading books..." })] })) : books && books.length > 0 ? (_jsx("div", { className: "table-responsive mt-3", children: _jsxs("table", { className: "table table-striped table-hover shadow-sm", children: [_jsx("thead", { className: "table-dark", children: _jsxs("tr", { children: [_jsx("th", { children: "Title" }), _jsx("th", { children: "Author" }), _jsx("th", { children: "Published Year" }), _jsx("th", { children: "Short Info" })] }) }), _jsx("tbody", { children: books.map(book => (_jsxs("tr", { children: [_jsx("td", { children: book.title }), _jsx("td", { children: book.author }), _jsx("td", { children: book.publishedYear }), _jsx("td", { children: book.shortInfo })] }, book.title))) })] }) })) : (isDataSearched && (_jsx("div", { className: "alert alert-warning mt-3", children: "No books found" })));
    return (_jsxs("div", { className: "container py-4", children: [_jsxs("div", { className: "text-center mb-4", children: [_jsx("h1", { className: "fw-bold", children: "\uD83D\uDCDA Book Discovery" }), _jsx("p", { className: "text-muted", children: "Type a book title or author name and click Search" })] }), _jsx("div", { className: "row justify-content-center", children: _jsx("div", { className: "col-md-8", children: _jsxs("div", { className: "input-group shadow-sm", children: [_jsx("input", { type: "text", className: "form-control", value: query, onChange: (e) => setQuery(e.target.value), placeholder: "e.g. Harry Potter" }), _jsx("button", { className: "btn btn-primary", onClick: () => populateBookData(query), children: "Search" })] }) }) }), _jsx("div", { className: "mt-4", children: contents })] }));
    async function populateBookData(search) {
        if (!query?.trim()) {
            setBooks([]);
            setIsDataSearched(false);
            return;
        }
        setLoading(true);
        try {
            const response = await searchBooks(search || query);
            setBooks(response);
        }
        finally {
            setIsDataSearched(true);
            setLoading(false);
        }
    }
}
export default App;
