const fs = require('fs');
const path = require('path');

const filePath = path.join(__dirname, 'course.json');
const data = fs.readFileSync(filePath, 'utf8');
const courses = JSON.parse(data);

const updatedCourses = courses.map(course => {
  return {
    ...course,
    description: `This is a detailed description for the ${course.name} course. It provides essential skills and knowledge required for this field.`
  };
});

fs.writeFileSync(filePath, JSON.stringify(updatedCourses, null, 2));
console.log('Successfully added description to all mock courses!');
