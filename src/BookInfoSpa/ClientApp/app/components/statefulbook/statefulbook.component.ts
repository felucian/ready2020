import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';
declare var bookInsertNewForm: boolean;
declare var bookListVisualRating: boolean;
declare var bookServiceUrl: string;

@Component({
    selector: 'app-statefulbook',
    templateUrl: './statefulbook.component.html',
    styleUrls: ['./statefulbook.component.css']
})

export class StatefulbookComponent {
    public books: Book[];
    public reviews: BookReviews[] | undefined;
    public currentBookId: number | undefined;
    public http: Http;
    public bookServiceUrl: string;
    public reviewLoadingFailure: boolean = false;
    public bookLoadingFailure: boolean = false;
    public displayView: boolean = true;
    public manageView: boolean = false;
    public newBook: Book | undefined;
    public schemaVersion: string | undefined;
    public binaryVersion: string | undefined;
    public bookInsertNewForm: boolean = false;
    public bookListVisualRating: boolean = false;
    private readonly bookserviceRoute = 'api/BookData/BookStateful/';
        
    constructor(http: Http) {
        this.http = http;
        this.books = [];
        this.bookInsertNewForm = bookInsertNewForm;
        this.bookListVisualRating = bookListVisualRating;
        this.bookServiceUrl = bookServiceUrl + this.bookserviceRoute;
        this.setEmptyNewBook();
        this.http.get(this.bookServiceUrl + 'BookList').subscribe(books => {
            this.books = books.json() as Book[];
            this.bookLoadingFailure = false;

            if (this.books != undefined && this.books.length > 0) {
                this.currentBookId = 1;
                this.getReviews(this.currentBookId);
            }

        }, error => {
            console.error(error);
            this.bookLoadingFailure = true;
        });

        this.http.get(this.bookServiceUrl + 'SchemaVersion').subscribe(version => {
            this.schemaVersion = version.json();
        }, error => {
            console.error(error);
        });

        this.http.get(this.bookServiceUrl + 'Health').subscribe(version => {
            this.binaryVersion = version.json();
        }, error => {
            console.error(error);
        });

    }

    public updateReviews(newBookId: number) {
        this.reviews = undefined;
        this.currentBookId = newBookId;
        this.getReviews(this.currentBookId);
    }

    public showDisplay() {
        this.displayView = true;
        this.manageView = false;
    }

    public showManage() {
        this.displayView = false;
        this.manageView = true;
    }

    public insertBook() {
        console.log(this.newBook);
        this.http.post(this.bookServiceUrl + 'InsertBook', this.newBook).subscribe(book => {
            this.books.push(book.json() as Book);
            this.setEmptyNewBook();
        }, error => {
            console.error(error);
            this.reviewLoadingFailure = true;
        });
    }

    private setEmptyNewBook() {
        this.newBook = { Title: "", Year: 0, Author: "", Price: 0, Id: 0};
    }

    private getReviews(bookId: number) {
        this.http.get(this.bookServiceUrl + 'Reviews/' + bookId).subscribe(reviews => {
            this.reviewLoadingFailure = false;
            this.reviews = reviews.json() as BookReviews[];
            console.log(this.reviews);
        }, error => {
            console.error(error);
            this.reviewLoadingFailure = true;
        });
    }
}

interface Book {
    Id: number;
    Title: string;
    Author: string;
    Year: number;
    Price: number;
    Pages?: number;
    IsValidForGift?: boolean;
}

interface BookReviews {
    Id: number;
    BookId: number;
    Author: string;
    Title: string;
    Description: string;
    Rating: number;
}