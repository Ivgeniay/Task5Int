export class BookRenderer {
    constructor(apiService, getParametersCallback) {
        this.expandedRows = new Set();
        this.apiService = apiService;
        this.getParametersCallback = getParametersCallback;
    }

    renderBooks(books) {
        const tbody = document.getElementById('booksTableBody');

        books.forEach(book => {
            const row = this.createBookRow(book);
            tbody.appendChild(row);
        });
    }

    createBookRow(book) {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td>${book.index}</td>
            <td>${book.isbn}</td>
            <td>${book.title}</td>
            <td>${book.authors.join(', ')}</td>
            <td>${book.publisher}</td>
        `;

        row.style.cursor = 'pointer';
        row.addEventListener('click', () => this.toggleBookDetails(book.index, row));

        return row;
    }

    async toggleBookDetails(bookIndex, row) {
        if (this.expandedRows.has(bookIndex)) {
            this.collapseBookDetails(bookIndex, row);
        } else {
            await this.expandBookDetails(bookIndex, row);
        }
    }

    async expandBookDetails(bookIndex, row) {
        try {
            const parameters = this.getParametersCallback();
            const bookDetails = await this.apiService.loadBookDetails(bookIndex, parameters);

            if (bookDetails) {
                this.showBookDetails(bookIndex, row, bookDetails);
                this.expandedRows.add(bookIndex);
            }
        } catch (error) {
        }
    }

    showBookDetails(bookIndex, row, bookDetails) {
        const detailsRow = document.createElement('tr');
        detailsRow.classList.add('book-details');
        detailsRow.innerHTML = `
            <td colspan="5" class="p-4">
                <div class="row">
                    <div class="col-md-2">
                        <div class="book-cover-container">
                            <img src="${bookDetails.coverImageUrl}" alt="Book cover" class="book-cover-image">
                            <div class="book-cover-overlay">
                                <div class="book-cover-title">${this.truncateText(bookDetails.book.title, 25)}</div>
                                <div class="book-cover-author">${this.truncateText(bookDetails.book.authors[0] || 'Unknown Author', 20)}</div>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-10">
                        <h5 class="mb-2">${bookDetails.book.title} <span class="text-muted">Paperback</span></h5>
                        <p class="mb-1">by <strong>${bookDetails.book.authors.join(', ')}</strong></p>
                        <p class="text-muted mb-3">${bookDetails.book.publisher}</p>
                        
                        <div class="mb-3">
                            <button class="btn btn-primary btn-sm">
                                <i class="bi bi-hand-thumbs-up"></i> ${bookDetails.likes}
                            </button>
                        </div>

                        ${bookDetails.reviews.length > 0 ? `
                            <h6>Reviews</h6>
                            <div class="reviews">
                                ${bookDetails.reviews.map(review => `
                                    <div class="mb-3">
                                        <p class="mb-1">${review.text}</p>
                                        <small class="text-muted">— ${review.author}, Rating: ${review.rating}/5</small>
                                    </div>
                                `).join('')}
                            </div>
                        ` : '<p class="text-muted">No reviews available.</p>'}
                    </div>
                </div>
            </td>
        `;

        row.after(detailsRow);
        row.classList.add('table-active');
    }

    collapseBookDetails(bookIndex, row) {
        const nextRow = row.nextElementSibling;
        if (nextRow && nextRow.classList.contains('book-details')) {
            nextRow.remove();
        }

        row.classList.remove('table-active');
        this.expandedRows.delete(bookIndex);
    }

    clearBooks() {
        document.getElementById('booksTableBody').innerHTML = '';
        this.expandedRows.clear();
    }

    truncateText(text, maxLength) {
        if (!text) return '';
        return text.length > maxLength ? text.substring(0, maxLength) + '...' : text;
    }
}