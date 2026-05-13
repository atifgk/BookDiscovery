import { useEffect, useState } from 'react';
import './App.css';
import { searchBooks } from "./services/Api";
import type Book from './models/Book';

function App() {
    const [books, setBooks] = useState<Book[]>();
    const [query, setQuery] = useState<string>();
    const [loading, setLoading] = useState<boolean>(false);
    const [isDataSearched, setIsDataSearched] = useState<boolean>(false);

    useEffect(() => {

    }, []);

    const contents = loading
        ? <p>Loading...</p>
        : books && books.length > 0
            ? (
                <table className="table table-striped" aria-labelledby="tableLabel">
                    <thead>
                        <tr>
                            <th>Title</th>
                            <th>Author</th>
                            <th>Published Year</th>
                            <th>Short Info</th>
                        </tr>
                    </thead>
                    <tbody>
                        {books.map(book => (
                            <tr key={book.title}>
                                <td>{book.title}</td>
                                <td>{book.author}</td>
                                <td>{book.publishedDate}</td>
                                <td>{book.shortInfo}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )
            : (isDataSearched ? <p>No books found</p> : <></>);

    return (
        <div>
            <h1>Search Books</h1>

            {/* Instruction label */}
            <p>Type a book title or author name and click Search</p>

            {/* Search input */}
            <div style={{ marginBottom: "10px" }}>
                <input
                    type="text"
                    value={query}
                    onChange={(e) => setQuery(e.target.value)}
                    placeholder="e.g. Harry Potter"
                />

                <button
                    onClick={() => populateBookData(query)}
                    style={{ marginLeft: "10px" }}
                >
                    Search
                </button>
            </div>

            {contents}
        </div>
    );

    async function populateBookData(search: string) {
        if (query) {
            setLoading(true);
            try {
                const response = await searchBooks(search);
                setBooks(response);
            } finally {
                setIsDataSearched(true);
                setLoading(false);
            }
        } else {
            setLoading(false);
            setIsDataSearched(false);
            setBooks([]);
        }
    }
}

export default App;