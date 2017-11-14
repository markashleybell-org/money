/// <binding ProjectOpened='watch' />
'use strict';

// Load gulp and the modules we need
var gulp = require('gulp'),
    sass = require('gulp-sass'),
    autoprefixer = require('gulp-autoprefixer');

// Set some configuration
var config = {
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

// Task to compile Sass
gulp.task('sass', function () {
    gulp.src(config.sass.sources)
        .pipe(sass(config.sass.options).on('error', sass.logError))
        .pipe(autoprefixer(config.autoprefixer))
        .pipe(gulp.dest(config.sass.root));
});

// This task watches all the source files for changes
gulp.task('watch', function () {
    gulp.watch(config.sass.sources, ['sass']);
});