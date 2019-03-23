package main

import (
	"os/exec"
)

func main() {
	exec.Command("runinstaller.bat").Start()
}

