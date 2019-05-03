/// <binding ProjectOpened='watch' />
'use strict';

const gulp = require('gulp');
const sass = require('gulp-sass');
const autoprefixer = require('gulp-autoprefixer');

const config = {
    sass: {
        root: './Content',
        sources: './Content/**/*.scss',
        options: {
            precision: 10,
            outputStyle: 'expanded',
            indentWidth: 4
        }
    },
    autoprefixer: {
        browsers: ['> 1%', 'last 2 versions', 'Firefox ESR', 'Opera 12.1'],
        cascade: false
    }
};

gulp.task('sass', () =>
    gulp.src(config.sass.sources)
        .pipe(sass(config.sass.options).on('error', sass.logError))
        .pipe(autoprefixer(config.autoprefixer))
        .pipe(gulp.dest(config.sass.root)));

gulp.task('watch', () => 
    gulp.watch(config.sass.sources, gulp.series('sass')));
