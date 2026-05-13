import { useEffect, useState } from 'react';
import './App.css';
import { searchBooks } from "./services/Api";
import type Book from './models/Book';

function App() {
    const [books, setBooks] = useState<Book[]>();
    const [query, setQuery] = useState<string>('');
    const [loading, setLoading] = useState<boolean>(false);
    const [isDataSearched, setIsDataSearched] = useState<boolean>(false);

    useEffect(() => {

    }, []);

    const contents = loading ? (
        <div className="text-center my-4">
            <div className="spinner-border text-primary" role="status"></div>
            <p className="mt-2">Loading books...</p>
        </div>
    ) : books && books.length > 0 ? (
        <div className="table-responsive mt-3">
            <table className="table table-striped table-hover shadow-sm">
                <thead className="table-dark">
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
                            <td>{book.publishedYear}</td>
                            <td>{book.shortInfo}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    ) : (
        isDataSearched && (
            <div className="alert alert-warning mt-3">
                No books found
            </div>
        )
    );

    return (
        <div className="container py-4">
            {/* Header */}
            <div className="text-center mb-4">
                <h1 className="fw-bold">📚 Book Discovery</h1>
                <p className="text-muted">
                    Type a book title or author name and click Search
                </p>
            </div>

            {/* Search Box */}
            <div className="row justify-content-center">
                <div className="col-md-8">
                    <div className="input-group shadow-sm">
                        <input
                            type="text"
                            className="form-control"
                            value={query}
                            onChange={(e) => setQuery(e.target.value)}
                            placeholder="e.g. Harry Potter"
                        />

                        <button
                            className="btn btn-primary"
                            onClick={() => populateBookData(query)}
                        >
                            Search
                        </button>
                    </div>
                </div>
            </div>

            {/* Results */}
            <div className="mt-4">
                {contents}
            </div>
        </div>
    );

    async function populateBookData(search?: string) {
        if (!query?.trim()) {
            setBooks([]);
            setIsDataSearched(false);
            return;
        }

        setLoading(true);

        try {
            const response = await searchBooks(search || query);
            setBooks(response);
        } finally {
            setIsDataSearched(true);
            setLoading(false);
        }
    }
}

export default App;